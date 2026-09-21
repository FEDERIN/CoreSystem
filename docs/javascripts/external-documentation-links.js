document.addEventListener("DOMContentLoaded", () => {
  document
    .querySelectorAll(
      'a[href^="https://federin.github.io/CoreSystem.Cache/"], a[href^="https://federin.github.io/CoreSystem.Idempotency/"]'
    )
    .forEach((link) => {
      link.target = "_blank";
      link.rel = "noopener noreferrer";
    });
});
