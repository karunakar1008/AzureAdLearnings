window.registerClickHandler = (dotnetHelper) => {
    document.addEventListener('click', (e) => {
        if (!e.target.closest('.profile-btn')) {
            dotnetHelper.invokeMethodAsync('HideProfileCard');
        }
    });
};