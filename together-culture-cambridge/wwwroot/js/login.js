
const validateField = (value, name) => {
    let hasError = false, error = "";

    if (!value) {
        hasError = true;
        error = "This field is required";
    } else {
        if (name === 'email') {
            if (!isValidEmail(value)) {
                hasError = true;
                error = "Please enter a valid email address"
            }
        }
    }

    return { hasError, error }
}


const sendLoginRequest = async (requestBody) => {
    const headers = generateHeaders();
    
    let location = window.location.href;
    let loginType = location.includes("EndUser") 
        ? "EndUser" :
        location.includes("Admin") ? "Admin" : "";
    
    if (!loginType) {
        throw new Error("Invalid login type");
    }
    let response = await fetch(`/${loginType}/Login`, {
        method: "POST",
        headers,
        body: JSON.stringify(requestBody)
    })

    let body = await response.json();
    return {
        statusCode: response.status,
        body,
        loginType
    }
}
$(document).ready(function () {

    
    $("#loginForm .base-input").on("input, change", function (e) {
        let { hasError, error } = validateField(e.target.value, e.target.name);
        let errorText = getErrorTextElement(e.target);
        
        if (errorText) {
            errorText.innerHTML = hasError ? error : "";
        }
        
        if (hasError) {
            $(this).addClass("with-error");
        } else { $(this).removeClass("with-error"); }
    })
    
    $(".login-btn").on("click", function (event) {
        event.preventDefault();
        
        let fieldsWithError = 0;
        let fields = {}
        Array.from($("#loginForm .base-input")).forEach(input => {
            let { hasError, error } = validateField(input.value, input.name);
            let errorText = getErrorTextElement(input);

            if (errorText) {
                errorText.innerHTML = hasError ? error : "";
            }
            
            
            if (hasError) {
                fieldsWithError += 1;
                input.classList.add("with-error");
            } else { 
                input.classList.remove("with-error");
                fields = {
                    ...fields,
                    [ input.name ] : input.value
                }
            }
        })
        
        if (fieldsWithError === 0) {
            $(this).parent().append("<div class='spinner'></div>")
            $(this).text("Signing you in...");
            
            let errorSection = $("#loginForm .error-section");
            const setErrorSection = (errorMessage) => {
                $(".btn-container .spinner").remove();
                $(this).text("Log in")

                errorSection.removeClass("d-none");
                errorSection.text(errorMessage);
                
                
                setTimeout(() => {
                    errorSection.addClass("d-none");
                    errorSection.text("")
                }, 3000)
            }
            errorSection.addClass("d-none")
            const defaultErrorMessage = "An unexpected error occurred while signing you in";
            
            $(this).attr("disabled", "true");
            sendLoginRequest(fields)
                .then(response => {
                    $(this).removeAttr("disabled");
                    if(response.statusCode === 200) {
                        console.log("Logged in:", response);
                        $(this).text("Log in")
                        if (response.loginType === "Admin") {
                            window.location.href = "/Admin/Dashboard";
                        } else if (response.loginType === "EndUser") {
                            window.location.href = "/EndUser/Dashboard";
                        }
                    } else {
                        setErrorSection(response.body.message ?? defaultErrorMessage)
                    }
                }).catch(error => {
                    console.log(error);
                     $(this).removeAttr("disabled")
                   
                
                    setErrorSection(defaultErrorMessage)
                })
        }
    })
})