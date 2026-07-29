let allRegistrations = [];

function fetchAndDrawRevenueChart() {
    fetch('/api/dashboardapi/revenues-by-subject')
        .then(response => {
            if (!response.ok) throw new Error('Failed to fetch revenue data');
            return response.json();
        })
        .then(data => {
            const message = document.getElementById('revenueChartMessage');
            message.hidden = data.length > 0;
            message.textContent = data.length === 0 ? 'No revenue data for this period.' : '';
            if (data.length === 0) {
                return;
            }
            const ctx = document.getElementById('revenuePieChart').getContext('2d');
            if (window.revenueChart) {
                window.revenueChart.destroy(); // Destroy the old chart if it exists
            }

            window.revenueChart = new Chart(ctx, {
                type: 'pie',
                data: {
                    labels: data.map(item => item.subjectName),
                    datasets: [{
                        label: 'Revenue by Subject',
                        data: data.map(item => item.revenue),
                        backgroundColor: generateColorArray(data.length),
                        borderColor: 'rgba(255,255,255,0.5)',
                        borderWidth: 2
                    }]
                },
                options: {
                    responsive: true,
                    plugins: {
                        legend: {
                            position: 'top',
                        },
                        tooltip: {
                            mode: 'index',
                            intersect: false,
                        },
                    }
                }
            });
        })
        .catch(() => {
            const message = document.getElementById('revenueChartMessage');
            message.hidden = false;
            message.textContent = 'Failed to load revenue data.';
        });
}

// Stable monochrome palette keeps charts consistent with the application theme.
function generateColorArray(num) {
    const palette = ['#fafafa', '#d4d4d4', '#a3a3a3', '#737373', '#525252', '#404040'];
    return Array.from({ length: num }, (_, index) => palette[index % palette.length]);
}

async function fetchCustomerStatistics() {
    try {
        const response = await fetch('/api/dashboardapi/customer-stats');
        if (!response.ok) {
            throw new Error(`HTTP error, status = ${response.status}`);
        }
        const data = await response.json();
        document.getElementById('newlyRegistered').textContent = data.newlyRegistered;
        document.getElementById('newlyBought').textContent = data.newlyBought;
    } catch (error) {
        document.getElementById('newlyRegistered').textContent = 'Failed to load';
        document.getElementById('newlyBought').textContent = 'Failed to load';
    }
}

async function updateChart(startDate, endDate) {
    if (!startDate || !endDate || isNaN(startDate.getTime()) || isNaN(endDate.getTime())) {
        alert("Invalid dates selected. Please select a valid date range.");
        return;
    }

    const errorMessage = document.getElementById('error-message');
    let orderData;
    let registrationData;
    try {
        [orderData, registrationData] = await Promise.all([
            fetchData(`/api/dashboardapi/order-count?startDate=${startDate.toISOString()}&endDate=${endDate.toISOString()}`),
            fetchData(`/api/dashboardapi/registration-count?startDate=${startDate.toISOString()}&endDate=${endDate.toISOString()}`)
        ]);
        errorMessage.textContent =
            orderData.length === 0 && registrationData.length === 0
                ? 'No reporting data for the selected period.'
                : '';
    } catch {
        errorMessage.textContent = 'Failed to load reporting data.';
        return;
    }
    // Process data to fill in missing dates
    const labels = [];
    const orderCounts = [];
    const registrationCounts = [];

    let currentDate = new Date(startDate);
    while (currentDate <= endDate) {
        const dateString = currentDate.toISOString().substring(0, 10);
        labels.push(dateString);

        const orderCountData = orderData.find(item => item.date === dateString);
        const registrationCountData = registrationData.find(item => item.date === dateString);

        orderCounts.push(orderCountData ? orderCountData.count : 0);
        registrationCounts.push(registrationCountData ? registrationCountData.count : 0);

        currentDate.setUTCDate(currentDate.getUTCDate() + 1);
    }

    const ctxOrder = document.getElementById('orderCountChart').getContext('2d');
    if (window.orderCountChart && typeof window.orderCountChart.destroy === 'function') {
        window.orderCountChart.destroy();
    }

    window.orderCountChart = new Chart(ctxOrder, {
        type: 'bar',
        data: {
            labels: labels,
            datasets: [
                {
                    label: 'Order Count',
                    data: orderCounts,
                    backgroundColor: 'rgba(250, 250, 250, 0.82)',
                    borderColor: '#fafafa',
                    borderWidth: 1
                },
                {
                    label: 'Registrated Count',
                    data: registrationCounts,
                    backgroundColor: 'rgba(115, 115, 115, 0.72)',
                    borderColor: '#a3a3a3',
                    borderWidth: 1
                }
            ]
        },
        options: {
            scales: {
                y: {
                    beginAtZero: true
                }
            }
        }
    });
}

async function fetchData(url) {
    const response = await fetch(url);
    if (!response.ok) {
        throw new Error("Failed to fetch data");
    }
    const data = await response.json();
    return data.map(item => ({
        date: item.date.substring(0, 10),
        count: item.count
    }));
}

function parseUtcDate(value) {
    return value ? new Date(`${value}T00:00:00Z`) : null;
}

function setupDateInputsAndChart() {
    let startDateInput = document.getElementById('startDate');
    let endDateInput = document.getElementById('endDate');
    if (!startDateInput.value) {
        let today = new Date();
        startDateInput.valueAsDate = new Date(today.setDate(today.getDate() - 7));
    }
    if (!endDateInput.value) {
        endDateInput.valueAsDate = new Date();
    }

    updateChart(parseUtcDate(startDateInput.value), parseUtcDate(endDateInput.value));
}

function onViewButtonClick() {
    var errorMessageDiv = document.getElementById("error-message");
    let startDateInput = document.getElementById('startDate');
    let endDateInput = document.getElementById('endDate');
    let startDate = parseUtcDate(startDateInput.value);
    let endDate = parseUtcDate(endDateInput.value);
    if (!startDate || !endDate || isNaN(startDate.getTime()) || isNaN(endDate.getTime()) || endDate < startDate) {
        errorMessageDiv.innerText = "Please select a valid date range.";
        return;
    } else {
        errorMessageDiv.innerText = "";
    }

    updateChart(startDate, endDate);
}



function fetchRegistrations() {
    fetch('/api/dashboardapi/registrations')
        .then(response => {
            if (!response.ok) throw new Error('Failed to fetch registrations');
            return response.json();
        })
        .then(data => {
            allRegistrations = data;
            updateRegistrations(allRegistrations);
            updateRegistrations(data);
            updatePieChart(data);
        })
        .catch(function () {
            document.getElementById('registrationList').innerHTML = '<div class="registration-item">Failed to load registrations.</div>';
        });
}

function filterRegistrations(status) {
    let filteredData = allRegistrations;
    if (status !== 'All') {
        filteredData = allRegistrations.filter(reg => reg.status === status);
    }
    updateRegistrations(filteredData);
    updateActiveButton(status);
}

function updateRegistrations(registrations) {
    const container = document.getElementById('registrationList');
    container.innerHTML = '';
    if (registrations.length === 0) {
        container.innerHTML = '<div class="registration-item">No registrations found.</div>';
        return;
    }
    registrations.forEach(reg => {
        const div = document.createElement('div');
        div.className = 'registration-item';
        const subject = document.createElement('span');
        subject.textContent = reg.subjectTitle ?? '';
        const status = document.createElement('span');
        status.className = 'status';
        status.textContent = reg.status ?? '';
        div.append(subject, status);
        container.appendChild(div);
    });
}

function updateActiveButton(status) {
    document.querySelectorAll('.filter-btn').forEach(btn => {
        btn.classList.remove('active');
        if (btn.textContent === status) {
            btn.classList.add('active');
        } else if (status === 'All' && btn.textContent === 'All') {
            btn.classList.add('active');
        }
    });
}

function updatePieChart(data) {
    const statusCounts = {
        Submitted: data.filter(item => item.status === 'Submitted').length,
        Cancelled: data.filter(item => item.status === 'Cancelled').length,
        Success: data.filter(item => item.status === 'Registrated').length
    };

    const ctx = document.getElementById('registrationStatusPieChart').getContext('2d');
    if (window.registrationStatusChart) {
        window.registrationStatusChart.destroy();
    }
    window.registrationStatusChart = new Chart(ctx, {
        type: 'pie',
        data: {
            labels: ['Submitted', 'Cancelled', 'Success'],
            datasets: [{
                label: 'Registration Status',
                data: [statusCounts.Submitted, statusCounts.Cancelled, statusCounts.Success],
                backgroundColor: [
                    '#fafafa',
                    '#737373',
                    '#bdbdbd'
                ],
                borderColor: [
                    '#ffffff',
                    '#a3a3a3',
                    '#e5e5e5'
                ],
                borderWidth: 1
            }]
        },
        options: {
            responsive: true,
            legend: {
                position: 'top',
            },
            title: {
                display: true,
                text: 'Registration Status'
            }
        }
    });

    fetchSubjects();
    fetchTotalRevenue();
    function fetchSubjects() {
        fetch('/api/dashboardapi/Subjects')
            .then(response => response.json())
            .then(data => {
                const select = document.getElementById('subjectSelect');
                data.forEach(subject => {
                    const option = document.createElement('option');
                    option.value = subject.id;
                    option.textContent = subject.title;
                    select.appendChild(option);
                });
            })
            .catch(function () {
                const select = document.getElementById('subjectSelect');
                select.innerHTML = '<option value="">Failed to load subjects</option>';
            });
    }

    function fetchTotalRevenue() {
        fetch('/api/dashboardapi/total-revenue')
            .then(response => {
                if (!response.ok) throw new Error('Failed to fetch total revenue');
                return response.json();
            })
            .then(data => {
                document.getElementById('totalRevenue').textContent = `${data.totalRevenue} USD`;
            })
            .catch(error => {
                document.getElementById('totalRevenue').textContent = 'Failed to load';
            });
    }

    window.fetchRevenueBySubject = function (subjectId) {
        if (!subjectId) return;
        fetch(`/api/dashboardapi/RevenuesBySubject?subjectId=${subjectId}`)

            .then(response => response.json())
            .then(data => {
                document.getElementById('subjectRevenueResult').innerHTML = `Revenue for Subject: ${data.totalRevenue} USD`;
            })
            .catch(error => {
                document.getElementById('subjectRevenueResult').innerHTML = 'Failed to load data.';
            });
    }

}
document.addEventListener('DOMContentLoaded', function () {
    fetchRegistrations();
    setupDateInputsAndChart();
    fetchCustomerStatistics();
    fetchAndDrawRevenueChart(); // Call this function on page load
});
