document.addEventListener("DOMContentLoaded", function () {
    const postsPerPage = 5; // Số blog hiển thị trên mỗi trang
    const posts = document.querySelectorAll('.blog-post');
    const paginationContainer = document.querySelector('.pagination');

    // Tạo và hiển thị các liên kết phân trang
    function displayPagination() {
        const numPages = Math.ceil(posts.length / postsPerPage);
        for (let i = 1; i <= numPages; i++) {
            const pageLink = document.createElement('li');
            const link = document.createElement('a');
            link.textContent = i;
            link.href = '#';
            link.addEventListener('click', function (event) {
                event.preventDefault();
                showPage(i);
                updateActiveLink(i);
            });
            pageLink.appendChild(link);
            paginationContainer.appendChild(pageLink);
        }
    }

    // Hiển thị chỉ các blog của trang hiện tại
    function showPage(pageNumber) {
        const startIndex = (pageNumber - 1) * postsPerPage;
        const endIndex = startIndex + postsPerPage;
        posts.forEach((post, index) => {
            if (index >= startIndex && index < endIndex) {
                post.style.display = 'flex';
            } else {
                post.style.display = 'none';
            }
        });
    }

    // Cập nhật liên kết phân trang đang được chọn
    function updateActiveLink(pageNumber) {
        const links = document.querySelectorAll('.pagination a');
        links.forEach(link => {
            if (link.textContent == pageNumber) {
                link.classList.add('active');
            } else {
                link.classList.remove('active');
            }
        });
    }

    // Mặc định hiển thị trang đầu tiên khi trang được tải
    displayPagination();
    showPage(1);
});
