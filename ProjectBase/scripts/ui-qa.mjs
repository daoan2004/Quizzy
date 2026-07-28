const endpoint = process.argv[2] ?? "http://127.0.0.1:9223";
const baseUrl = process.argv[3] ?? "http://localhost:5152";
const testEmail = process.argv[4];
const testPassword = process.argv[5];

const targets = [
  ["/", "home"],
  ["/Subjects", "subjects"],
  ["/Blogs", "blogs"],
  ["/Practice", "practice"],
  ["/Practice/NewPractice", "new-practice"],
  ["/SimulationExam", "simulation"],
  ["/QuizReview", "quiz-review"],
  ["/Dashboard", "dashboard"],
  ["/MyRegistrations", "registrations"],
  ["/Account/ResetPasswordRequest", "reset-password"],
];

const sleep = (milliseconds) => new Promise((resolve) => setTimeout(resolve, milliseconds));

async function waitForBrowser() {
  for (let attempt = 0; attempt < 30; attempt += 1) {
    try {
      const response = await fetch(`${endpoint}/json/version`);
      if (response.ok) return;
    } catch {
      // Chrome is still starting.
    }
    await sleep(250);
  }
  throw new Error(`Chrome DevTools endpoint did not start at ${endpoint}`);
}

await waitForBrowser();
const pageResponse = await fetch(`${endpoint}/json/new?${encodeURIComponent("about:blank")}`, {
  method: "PUT",
});
const page = await pageResponse.json();
const socket = new WebSocket(page.webSocketDebuggerUrl);

let sequence = 0;
const pending = new Map();
const consoleErrors = [];
const pageErrors = [];

socket.addEventListener("message", (event) => {
  const message = JSON.parse(event.data);
  if (message.id && pending.has(message.id)) {
    const { resolve, reject } = pending.get(message.id);
    pending.delete(message.id);
    if (message.error) reject(new Error(message.error.message));
    else resolve(message.result);
    return;
  }

  if (message.method === "Runtime.exceptionThrown") {
    pageErrors.push(message.params.exceptionDetails.text);
  }

  if (message.method === "Runtime.consoleAPICalled" && message.params.type === "error") {
    consoleErrors.push(message.params.args.map((item) => item.value ?? item.description).join(" "));
  }
});

await new Promise((resolve, reject) => {
  socket.addEventListener("open", resolve, { once: true });
  socket.addEventListener("error", reject, { once: true });
});

function command(method, params = {}) {
  const id = ++sequence;
  return new Promise((resolve, reject) => {
    const timeout = setTimeout(() => {
      pending.delete(id);
      reject(new Error(`CDP command timed out: ${method}`));
    }, 10000);

    pending.set(id, {
      resolve: (value) => {
        clearTimeout(timeout);
        resolve(value);
      },
      reject: (error) => {
        clearTimeout(timeout);
        reject(error);
      },
    });
    socket.send(JSON.stringify({ id, method, params }));
  });
}

async function evaluate(expression) {
  const result = await command("Runtime.evaluate", {
    expression,
    returnByValue: true,
    awaitPromise: true,
  });
  return result.result.value;
}

async function navigate(path) {
  await command("Page.navigate", { url: `${baseUrl}${path}` });
  await sleep(1200);
}

await command("Page.enable");
await command("Runtime.enable");
await command("Log.enable");
await command("Network.enable");
await command("Network.clearBrowserCookies");
await command("Emulation.setDeviceMetricsOverride", {
  width: 375,
  height: 812,
  deviceScaleFactor: 1,
  mobile: true,
  screenWidth: 375,
  screenHeight: 812,
});
await command("Emulation.setTouchEmulationEnabled", { enabled: true, maxTouchPoints: 1 });

const results = [];
for (const [path, name] of targets) {
  console.error(`QA ${path}`);
  consoleErrors.length = 0;
  pageErrors.length = 0;
  await navigate(path);

  const metrics = await evaluate(`(() => ({
    viewportWidth: window.innerWidth,
    documentWidth: document.documentElement.scrollWidth,
    bodyWidth: document.body.scrollWidth,
    horizontalOverflow: document.documentElement.scrollWidth > window.innerWidth + 1,
    overflowElements: [...document.querySelectorAll("body *")]
      .map(element => ({ element, rect: element.getBoundingClientRect() }))
      .filter(item => !item.element.closest(".swiper"))
      .filter(item => item.rect.right > innerWidth + 1 || item.rect.left < -1 || item.rect.width > innerWidth + 1)
      .slice(0, 8)
      .map(item => ({
        tag: item.element.tagName,
        id: item.element.id,
        className: String(item.element.className).slice(0, 120),
        left: Math.round(item.rect.left),
        right: Math.round(item.rect.right),
        width: Math.round(item.rect.width)
      })),
    title: document.title
  }))()`);

  results.push({
    path,
    name,
    ...metrics,
    consoleErrors: [...consoleErrors],
    pageErrors: [...pageErrors],
  });
}

await navigate("/");
const interactions = await evaluate(`(async () => {
  const result = {};
  const toggler = document.querySelector(".navbar-toggler");
  result.togglerVisible = Boolean(toggler && getComputedStyle(toggler).display !== "none");
  toggler?.click();
  await new Promise(resolve => setTimeout(resolve, 450));
  const collapse = document.querySelector("#mainNavbar");
  result.menuExpanded = toggler?.getAttribute("aria-expanded") === "true";
  result.menuShown = Boolean(collapse?.classList.contains("show"));
  toggler?.click();
  await new Promise(resolve => setTimeout(resolve, 450));
  result.menuCollapsed = toggler?.getAttribute("aria-expanded") === "false";

  const loginButton = document.querySelector("#loginBtn");
  loginButton?.click();
  await new Promise(resolve => setTimeout(resolve, 450));
  const modal = document.querySelector("#loginModal");
  const modalContent = modal?.querySelector(".modal-content");
  const rect = modalContent?.getBoundingClientRect();
  result.loginModalShown = Boolean(modal?.classList.contains("show"));
  result.loginModalFitsViewport = Boolean(rect && rect.left >= 0 && rect.right <= innerWidth && rect.top >= 0 && rect.height <= innerHeight);
  result.loginEmailAccessibleName = modal?.querySelector("#inputUserNameLogin")?.getAttribute("aria-label") ?? "";
  modal?.querySelector(".modal-close")?.click();
  await new Promise(resolve => setTimeout(resolve, 350));
  result.loginModalClosedByButton = !modal?.classList.contains("show");
  document.querySelector("#registerBtn")?.click();
  await new Promise(resolve => setTimeout(resolve, 450));
  const registerModal = document.querySelector("#registerModal");
  const registerRect = registerModal?.querySelector(".modal-content")?.getBoundingClientRect();
  result.registerModalShown = Boolean(registerModal?.classList.contains("show"));
  result.registerModalFitsViewport = Boolean(registerRect && registerRect.left >= 0 && registerRect.right <= innerWidth && registerRect.height <= innerHeight);
  registerModal?.querySelector(".modal-close")?.click();
  await new Promise(resolve => setTimeout(resolve, 350));
  result.registerModalClosedByButton = !registerModal?.classList.contains("show");
  return result;
})()`);

let authenticated = null;
if (testEmail && testPassword) {
  const loginResult = await evaluate(`(async () => {
    const response = await fetch("/Account/Login", {
      method: "POST",
      headers: { "Content-Type": "application/json" },
      body: JSON.stringify(${JSON.stringify({ email: testEmail, password: testPassword })})
    });
    return await response.json();
  })()`);

  const authenticatedTargets = [
    ["/Practice", "practice"],
    ["/Practice/NewPractice", "new-practice"],
    ["/SimulationExam", "simulation"],
    ["/QuizReview", "quiz-review"],
    ["/QuizReview/Detail?id=801", "quiz-review-detail"],
    ["/Quiz/Handle?UserID=10003&PracticeID=801&IsPractice=true", "quiz-handle"],
    ["/MyRegistrations", "registrations"],
  ];
  const authenticatedRoutes = [];

  for (const [path, name] of authenticatedTargets) {
    consoleErrors.length = 0;
    pageErrors.length = 0;
    await navigate(path);
    const metrics = await evaluate(`(() => ({
      viewportWidth: innerWidth,
      documentWidth: document.documentElement.scrollWidth,
      horizontalOverflow: document.documentElement.scrollWidth > innerWidth + 1,
      title: document.title,
      hasUserMenu: Boolean(document.querySelector("#userMenu")),
      quizQuestionButtons: document.querySelectorAll(".question-button").length,
      hasVisibleErrorState: [...document.querySelectorAll(".alert-danger, .table-state-message, .form-state-message")]
        .some(element => getComputedStyle(element).display !== "none" && element.textContent.trim().length > 0)
    }))()`);
    authenticatedRoutes.push({
      path,
      name,
      ...metrics,
      consoleErrors: [...consoleErrors],
      pageErrors: [...pageErrors],
    });
  }

  await navigate("/");
  const modalChecks = await evaluate(`(async () => {
    const result = {};
    document.querySelector("#profileBtn")?.click();
    await new Promise(resolve => setTimeout(resolve, 600));
    const profileModal = document.querySelector("#profileModal");
    result.profileModalShown = profileModal?.classList.contains("show") ?? false;
    profileModal?.querySelector(".modal-close")?.click();
    await new Promise(resolve => setTimeout(resolve, 350));
    result.profileModalClosedByButton = !profileModal?.classList.contains("show");
    document.querySelector("#changePasswordBtn")?.click();
    await new Promise(resolve => setTimeout(resolve, 450));
    result.changePasswordModalShown = document.querySelector("#changePasswordModal")?.classList.contains("show") ?? false;
    result.changePasswordModalFitsViewport = (() => {
      const rect = document.querySelector("#changePasswordModal .modal-content")?.getBoundingClientRect();
      return Boolean(rect && rect.left >= 0 && rect.right <= innerWidth && rect.height <= innerHeight);
    })();
    const changePasswordModal = document.querySelector("#changePasswordModal");
    changePasswordModal?.querySelector(".modal-close")?.click();
    await new Promise(resolve => setTimeout(resolve, 350));
    result.changePasswordModalClosedByButton = !changePasswordModal?.classList.contains("show");
    return result;
  })()`);

  await navigate("/Practice/NewPractice");
  const practiceValidation = await evaluate(`(() => {
    document.querySelector(".submit-button")?.click();
    return {
      blankNameRejected: (document.querySelector(".alerttestname")?.textContent ?? "").trim().length > 0
    };
  })()`);

  await navigate("/MyRegistrations");
  const registrationModals = await evaluate(`(async () => {
    await new Promise(resolve => setTimeout(resolve, 800));
    const result = {};
    document.querySelector(".btn-cancel")?.click();
    await new Promise(resolve => setTimeout(resolve, 250));
    result.cancelModalShown = getComputedStyle(document.querySelector("#myModal")).display !== "none";
    document.querySelector("#closeModal")?.click();
    document.querySelector(".btn-pay")?.click();
    await new Promise(resolve => setTimeout(resolve, 250));
    result.paymentModalShown = getComputedStyle(document.querySelector("#payModal")).display !== "none";
    document.querySelector("#closePayModal")?.click();
    return result;
  })()`);

  authenticated = {
    loginSucceeded: loginResult.success === true,
    routes: authenticatedRoutes,
    modalChecks,
    practiceValidation,
    registrationModals,
  };
}

console.log(JSON.stringify({
  viewport: { width: 375, height: 812 },
  routes: results,
  interactions,
  authenticated,
}, null, 2));

socket.close();
