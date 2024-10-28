/* Make sure that the user can scoll when hover to the invisible scoll bar */

const container = document.querySelector('.subject_categories');
container.addEventListener('wheel', (e) => {
    e.preventDefault();
    container.scrollLeft += e.deltaY;
});



document.addEventListener("DOMContentLoaded", function () {
    const postsPerPage = 6; // Number of subjects to display per page
    const posts = document.querySelectorAll('.subject-item:not(.placeholder)');
    const paginationContainer = document.querySelector('.pagination');
    const placeholder = document.querySelector('.placeholder');

    // Create and display pagination links
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

    // Display only the subjects of the current page
    function showPage(pageNumber) {
        const startIndex = (pageNumber - 1) * postsPerPage;
        const endIndex = startIndex + postsPerPage;
        const numPages = Math.ceil(posts.length / postsPerPage);
        posts.forEach((post, index) => {
            if (index >= startIndex && index < endIndex) {
                post.style.display = 'flex';
            } else {
                post.style.display = 'none';
            }
        });

        // Adjust the placeholder visibility if the last page has an odd number of items
        if (pageNumber === numPages && (posts.length % 2 !== 0)) {
            placeholder.style.display = 'block';
        } else {
            placeholder.style.display = 'none';
        }
    }

    // Update the active pagination link
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

    // Display the first page by default when the page loads
    displayPagination();
    showPage(1);
});


// JS for popup




