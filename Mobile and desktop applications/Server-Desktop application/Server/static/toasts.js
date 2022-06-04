const toastList = document.getElementById('toast_list');


export function createToast(title, body) {
    const toast = document.createElement("div");
    toast.setAttribute("class", "toast");
    toast.setAttribute("role", "alert");
    toast.setAttribute("aria-live", "assertive");
    toast.setAttribute("aria-atomic", "true");

    let content = '<div class="toast-header">';
    content += '<strong class="me-auto">' + title + '</strong>';
    content += '<button type="button" class="btn-close" data-bs-dismiss="toast" aria-label="Close"></button>';
    content += '</div>';
    content += '<div class="toast-body">';
    content += body;
    content += '</div>';  
    toast.innerHTML = content;
    toastList.appendChild(toast);

    toast.addEventListener('hidden.bs.toast', function () {
        toast.remove();
    })

    const toastInstance = new bootstrap.Toast(toast)
    toastInstance.show()
}