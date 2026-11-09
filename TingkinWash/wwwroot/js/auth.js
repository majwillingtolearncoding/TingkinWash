
document.addEventListener("DOMContentLoaded", function () {

    // Show / hide passwords
    document.querySelectorAll("[data-toggle-password]").forEach(button => {
        button.addEventListener("click", function () {
            const inputId = this.getAttribute("data-toggle-password");
            const input = document.getElementById(inputId);

            if (!input) return;

            const isPassword = input.type === "password";
            input.type = isPassword ? "text" : "password";
            this.textContent = isPassword ? "Hide" : "Show";
            this.setAttribute(
                "aria-label",
                isPassword ? "Hide password" : "Show password"
            );
        });
    });

    // Password strength indicator on registration page
    const passwordInput = document.getElementById("registerPassword");
    const strengthBar = document.getElementById("strengthBar");
    const strengthText = document.getElementById("strengthText");

    if (passwordInput && strengthBar && strengthText) {
        passwordInput.addEventListener("input", function () {
            const password = this.value;
            let score = 0;

            if (password.length >= 8) score++;
            if (/[a-z]/.test(password)) score++;
            if (/[A-Z]/.test(password)) score++;
            if (/[0-9]/.test(password)) score++;
            if (/[^a-zA-Z0-9]/.test(password)) score++;

            const levels = [
                { width: "0%", color: "#DC3545", text: "Password strength" },
                { width: "20%", color: "#DC3545", text: "Very weak" },
                { width: "40%", color: "#F59E0B", text: "Weak" },
                { width: "60%", color: "#EAB308", text: "Fair" },
                { width: "80%", color: "#22A06B", text: "Good" },
                { width: "100%", color: "#16834A", text: "Strong" }
            ];

            const level = levels[score];

            strengthBar.style.width = level.width;
            strengthBar.style.backgroundColor = level.color;
            strengthText.textContent = level.text;
        });
    }

    // Form loading state
    document.querySelectorAll(".auth-form").forEach(form => {
        form.addEventListener("submit", function (event) {
            if (!this.checkValidity()) return;

            const button = this.querySelector(".auth-button");

            if (!button) return;

            button.disabled = true;

            const buttonText = button.querySelector(".button-text");

            if (buttonText) {
                buttonText.textContent = "PLEASE WAIT...";
            }
        });
    });

});
