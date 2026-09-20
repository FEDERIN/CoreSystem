document.addEventListener("DOMContentLoaded", () => {
  document
    .querySelectorAll('a[href^="https://federin.github.io/CoreSystem.Cache/"]')
    .forEach((link) => {
      link.target = "_blank";
      link.rel = "noopener noreferrer";
    });
});
