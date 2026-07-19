// Apply dark background to all select elements
document.addEventListener('DOMContentLoaded', function() {
    const selects = document.querySelectorAll('select');
    selects.forEach(select => {
        // Get the computed --select-bg value from CSS variables
        const selectBg = getComputedStyle(document.documentElement).getPropertyValue('--select-bg').trim();
        const inputText = getComputedStyle(document.documentElement).getPropertyValue('--input-text').trim();
        
        select.style.backgroundColor = selectBg || '#2a2f2b';
        select.style.color = inputText || '#e5efe0';
    });
});

// Re-apply when theme changes
document.addEventListener('themeChanged', function() {
    setTimeout(() => {
        const selects = document.querySelectorAll('select');
        selects.forEach(select => {
            const selectBg = getComputedStyle(document.documentElement).getPropertyValue('--select-bg').trim();
            const inputText = getComputedStyle(document.documentElement).getPropertyValue('--input-text').trim();
            
            select.style.backgroundColor = selectBg || '#2a2f2b';
            select.style.color = inputText || '#e5efe0';
        });
    }, 100);
});
