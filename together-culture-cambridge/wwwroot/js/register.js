

const containsDigits = (string) => {
    return /\d/.test(string);
}

const containsSpecialChars = (string) => {
    let format = /[!@#$%^&*()_+\-=\[\]{};':"\\|,.<>\/?]+/;
    return format.test(string);
}
let nonRequiredFields = [ "discountCode" ]


const validateField = (value, fieldName) => {
    let hasError = false, error = "";
    
    let requiredField = nonRequiredFields.indexOf(fieldName) === -1;

    if (!value) {
       if (requiredField) {
           hasError = true;
           error = "This field is required"
       }
    } else {
        if (fieldName === "email" && !isValidEmail(value)) {
            hasError = true;
            error = "Please enter a valid email address"
        } else if (fieldName === "password") {
            if (value.length < 8) {
                hasError = true;
                error = "Password must be at least 8 characters"
            } else if (value.toLowerCase() === value) {
                hasError = true;
                error = "Password should contain at least one uppercase letter"
            } else if (!containsDigits(value)) {
                hasError = true;
                error = "Password must contain at least one digit letter"
            } else if (!containsSpecialChars(value)) {
                hasError = true;
                error = "Password must contain at least one special character"
            }
        } else if (fieldName === "confirmPassword") {
            let passwordField = $("#memberForm input[name='password']");
            let passwordValue = passwordField.val();
            console.log({ passwordValue, passwordField, value });
            if (passwordValue !== value) {
                hasError = true;
                error = "Passwords do not match"
            }
        }
    }
    
    
    return { hasError, error };
}

const checkTestItem = (element, passedTest) => {
    if (passedTest) {
        if (!element.hasClass("passed-test")) {
            element.addClass("passed-test");
        }
    } else {
        if (element.hasClass("passed-test")) {
            element.removeClass("passed-test");
        }
    }
}


const setPasswordRequirementState = (value) => {
    let passedLengthTest = value.length > 8;
    let lengthTest = $(".password-requirements .length-test");
    checkTestItem(lengthTest, passedLengthTest);

    let passedUppercaseTest = value.toLowerCase() !== value;
    let uppercaseTest = $(".password-requirements .uppercase-test");

    checkTestItem(uppercaseTest, passedUppercaseTest);

    let passedDigitsTest = containsDigits(value);
    let digitsTest = $(".password-requirements .digit-test");

    checkTestItem(digitsTest, passedDigitsTest);

    let passedSpecialCharsTest = containsSpecialChars(value);
    let specialCharsTest = $(".password-requirements .special-char-test");

    checkTestItem(specialCharsTest, passedSpecialCharsTest);
}
$(".form-section .base-input").on('change, input', function (event) {
    let value = $(this).val();
    console.log({ value });

    let errorText = getErrorTextElement(event.target);
    let { hasError, error } = validateField(event.target.value, event.target.name);
    if (event.target.name === "password") {
       setPasswordRequirementState(value)
    } else {
        if (errorText) {
            errorText.innerHTML = hasError ? error : "";
        } 
    }
   

    //console.log({ error, name: event.target.name });
   


    
})


const checkInputFields = () => {
    let validated = true;
    Array.from($("#guestForm .base-input")).forEach(input => {
        console.log("Value:", input.value);
        let errorText = getErrorTextElement(input);
        let { hasError, error } = validateField(input.value, input.name);
        
        if (hasError && validated) { 
            validated = false
        }
        if (input.name === "password") {
            setPasswordRequirementState(value)
        } else {
            if (errorText) {
                errorText.innerHTML = hasError ? error : "";
            }
        }
        
        if (hasError) {
            input.classList.add("with-error");
        } else { input.classList.remove("with-error"); }
    })
    
    return validated;
}
$("#guestForm .base-input").on('change, input', function(event){
    let value = $(this).val();
    console.log(value);
    let errorText = getErrorTextElement(event.target);
    
    let { hasError, error } = validateField(event.target.value, event.target.name);
    
    if (errorText) {
        errorText.innerHTML = hasError ? error : "";
    }
    
    if (hasError) {
        $(this).addClass("with-error");
    } else { $(this).removeClass("with-error"); }
    
    let fieldsWithError = Array.from($("#guestForm .base-input")).filter(field => !field.value || field.classList.contains("with-error"));
    let submitBtn = $("#guestForm button.create-btn")
    
    
    
    if (submitBtn) {
        submitBtn[0].disabled = fieldsWithError.length > 0;
    }
})





const sendEmail = async (email) => {
    const headers = generateHeaders();
    let response = await fetch(`/EndUser/SendEmail/${email}`, {
        method: "POST",
        headers
    })

    let body = await response.json();
    return {
        statusCode: response.status,
        body
    }
}

const validateDiscount = async (code) => {
    let requestBody = { code }
    console.log({ requestBody }); 
    const headers = generateHeaders();
    let response = await fetch(`/Discount/Verify`, {
        method: "POST",
        headers,
        body: JSON.stringify(requestBody),
    })
    let body = await response.json();
    return {
        statusCode: response.status,
        body
    } 
}
/*console.log("Sending email.....")
sendEmail("kuteyisamueldev@gmail.com")
    .then(res => {
        console.log("Response:", res);
    }).catch(err => {
        console.log("Error:", err)
    });*/
const createUser = async  (requestBody, userType) => {
    const headers = generateHeaders();
   
    let response = await fetch(`/EndUser/Create/${userType}`, {
        method: "POST",
        headers: headers,
        body: JSON.stringify(requestBody)
    })
    
    
    let body = await response.json();
    return {
        statusCode: response.status,
        body
    }
    //console.log({ response })
   // return await response.json();
}

$(".membership-list .price-item").on("click", function () {
    let items =  $(".membership-list .price-item");
    let elIndex = items.index(this);
    console.log(elIndex);
    
    for (let index = 0; index < items.length; index++) {
        let membershipItem = $(".membership-list .membership-item");
        let el = membershipItem[index];
        //console.log(el);
        if (index === elIndex) {
            el.classList.toggle("selected")
        } else {
            el.classList.remove("selected");
        }
    }
    
    
  
    
})
$('#guestForm .create-btn').on('click', function(e) {
    e.preventDefault();
    if (checkInputFields()) {
     //console.log({ target })
     
     let fields = Array.from($("#guestForm .base-input"));
     
     let fieldsWithError = fields.filter(field => field.classList.contains("with-error"));
     if (fieldsWithError.length > 0) {
         return;
     }
     
     let requestObject = {};
    
    
     fields.forEach(field => {
         requestObject = {
             ...requestObject,
             [field.name] : field.value
         }
     })
     
     $("#guestForm .btn-container .spinner").remove();
     $(this).parent().append(`<div class="spinner"></div>`);
     
     
     const resetButton = () => {
         $(this).removeClass('button-success');
         if (!$(this).hasClass("button-secondary")) {
             $(this).addClass("button-secondary");
         }
     }
     
     console.log(requestObject);
     e.target.disabled = true;
     let errorSection = $("#guestForm .error-section");
     errorSection.text("");
     if (!errorSection.hasClass("d-none")) {
         errorSection.addClass("d-none");
     }
     
     const resetTimeout = 3000;
     createUser(requestObject, "Guest")
         .then(res => {
             $("#guestForm .btn-container .spinner").remove();
             e.target.disabled = false;
             
             
             if (res.statusCode === 200){
                 $(this).removeClass("button-secondary");
                 $(this).addClass("button-success");
                 $(this).text("Account created!")
                 
                 
                 setTimeout(() => {
                     resetButton()
                     $(this).text("Create Guest Account")
                 }, resetTimeout)
             } else {
                 errorSection.text(res.body.message);
                 errorSection.removeClass("d-none");
                 
                 setTimeout(() => {
                     errorSection.addClass("d-none");
                     errorSection.text("")
                 }, resetTimeout);
             }
             console.log(res); 
         })
         .catch(err => {
             console.log(err);
             $("#guestForm .btn-container .spinner").remove();
             e.target.disabled = false;
             errorSection.removeClass("d-none");
             
             errorSection.text("An error occurred while creating your account");
             
         })
    }
   
  
    //console.log(requestObject)
})

const setHeightVariables = (section) => {
    let header = section.querySelector(".section-header");
    let body = section.querySelector(".section-body");


    // console.log(section);
    if ( header && body ) {
        let fullHeight = header.clientHeight + body.clientHeight;
        section.style.cssText = `--closed-height:${header.clientHeight}px;--open-height:${fullHeight}px;`;
    }
}
const setSectionHeight =() => {
    let registrationSections = $(".form-sections .form-section");
    if (registrationSections.length > 0) {
        Array.from(registrationSections).forEach(section => {
            setHeightVariables(section);
        })
    }
}



const detectResize = () => {
    const resizeObserver = new ResizeObserver((entries) => {
        for (const entry of entries) {
            if (entry.contentBoxSize) {
               // console.log("Resized")
               // entry.target.style.cssText = `--closed-height:${entry.contentRe}`
                setHeightVariables(entry.target);
            }
        }
    })

    let registrationSections = $(".form-sections .form-section")
    
   Array.from(registrationSections).forEach(section => {
        resizeObserver.observe(section)
    })
    
}

const disableNextSteps = () => {
    Array.from(sections).forEach((section, index) => {
        if (index > savedStep && !section.classList.contains("disabled")) {
            section.classList.add("disabled");
        }
    })
}

const moveToStep = (stepIndex) => {
    for (let index = 0; index < sectionHeaders.length; index++) {
        let container = sectionHeaders[index].parentElement
        if (index === stepIndex) {
            container.classList.toggle("open");
            container.classList.remove("disabled");


        } else {
            // console.log({ section: sectionHeaders[index] })
            container.classList.remove("open");
        }
    }
}



let savedStep = 0;
let sections = $(".form-sections .form-section");


detectResize();

// console.log(activeStep);
const setMemberFormState = () => {
    setSectionHeight();
    
    for (let index = 0; index < sections.length; index++) {
        let activeStep = sections[index]
        if (index === savedStep) {
            if (!activeStep.classList.contains("open")) {
                activeStep.classList.add("open")
            }
        } else {
            activeStep.classList.remove("open")
        }
    }

    disableNextSteps();
}

setMemberFormState();


let registrationTypes = $(".registration-box .type-list .reg-type");
registrationTypes.on("click", function (e) {
    if ($(this).hasClass("active")) return;
    let typeIndex = registrationTypes.index($(this));
    console.log("Event:", e);
    
    let regType = $(this).attr("data-type");
    console.log({ regType })
    Array
        .from(registrationTypes)
        .forEach((registrationType, index) => {
            if (index === typeIndex) {
                registrationType.classList.add("active");
            } else {
                registrationType.classList.remove("active");
            }
        })
    
    let guestForm = $("#guestForm");
    let memberForm = $("#memberForm");
    switch (regType) {
        case "guest":
            guestForm.removeClass("d-none");
            memberForm.addClass("d-none");
            return
        case "member":
            guestForm.addClass("d-none");
            memberForm.removeClass("d-none");
            
            setMemberFormState();
            return
    }
})


let sectionHeaders = $(".form-section .section-header");
let savedUserData = {};
let verificationCode = { code: '', createdAt: new Date().toISOString() };

sectionHeaders.on("click", function () {
    let disabled = $(this).parent().hasClass("disabled");
    if (disabled) return;
    let headerIndex = sectionHeaders.index(this);
    
    moveToStep(headerIndex);
})


$(".discount-btn").on("click", function (e) {
    e.preventDefault();
    $(this).attr("disabled", true);
    $(this).text("Applying....")
    
    
    let discountField = $("input[name='discountCode']")[0];
    console.log(discountField);
    let value = discountField.value;
    
    let feedbackSection = $(".discount-section .feedback-section");
    
    const resetSection = () => {
        feedbackSection.addClass("d-none");
        feedbackSection.removeClass("error-section");
        feedbackSection.removeClass("success-section");
        feedbackSection.text("")

        setSectionHeight()
    }
    
    validateDiscount(value)
        .then((res) => {
            console.log(res);
            feedbackSection.removeClass("d-none");
            $(this).text("Apply")
            $(this).attr("disabled", false);
            setTimeout(() => { setSectionHeight() }, 0)
           if (res.statusCode !== 200) {
               feedbackSection.addClass("error-section");
               feedbackSection.text(res.body.message);

               setTimeout(function () {
                   resetSection();
               }, 3000)
           } else {
               let discount = res.body.value['discount'];
               savedUserData.discount = discount;
               feedbackSection.addClass("success-section");
               feedbackSection.text(`${ discount['percentage'] * 100 }% discount applied.`)
           }

           
        }).catch(() => {
            $(this).text("Apply")
            $(this).attr("disabled", false);
             setTimeout(() => { setSectionHeight() }, 0)
        
            feedbackSection.removeClass("d-none");
            feedbackSection.addClass("error-section");
            feedbackSection.text("Error applying discount");
            
            
            setTimeout(function () {
                resetSection();
            }, 3000)
        })
})
$(".code-resend-btn").on("click", function (e) {
    e.preventDefault();
    $(this).text("Resending...")
    sendEmail(savedUserData['personalInfo'].email)
        .then((res) => {
            verificationCode = res.body.value;
            $(this).text("Code resent!");
            $(this).attr("disabled", true);
            
            let element = $(this);
            setTimeout(function () {
              let timeout = 30;
              
              const updateButtonText = () => {
                  
                  if (timeout <= 0) {
                      element.text("Resend code")
                      element.attr("disabled", false);
                      return;
                  }
                  element.text(`Resend code in ${timeout}`)
                  
                  
                  setTimeout(function () {
                      timeout -= 1;
                      updateButtonText();
                  }, 1000)
              }
              
              updateButtonText();
            }, 2000)
        }).catch(() => {
            $(this).text("Error sending code")
            setTimeout(() => {
                $(this).text("Resend code");
            }, 3000)
    })
})
$("#memberForm .next-step-btn").on("click", function (e) {
    e.preventDefault();
    //console.log("Next button clicked");
    
    let buttonIndex = $("#memberForm .next-step-btn").index(this);
    console.log({ buttonIndex });
    if (buttonIndex + 1 >= sections.length) {
        
    } else {
        let section = e.target.closest(".form-section");
       // console.log(section);
        if (section) {
            let fields = section.querySelectorAll(".base-input");
            let fieldsWithError = 0;
            let sectionData = {};
            let sectionKey = section.dataset.section;
            fields.forEach(field => {
                let { hasError, error } = validateField(field.value, field.name);
                let errorSection = getErrorTextElement(field);
                
                if (field.name === "verificationCode" && !hasError) {
                    console.log({
                        code: verificationCode.code,
                        value: field.value
                    })
                    if (field.value !== verificationCode.code) {
                        hasError = true;
                        error = "Incorrect code"
                    } else {
                        let creationDate = new Date(verificationCode.createdAt);
                        let currentDate = new Date();
                        let minuteDifference = (currentDate.getTime() - creationDate.getTime()) / (1000 * 60 * 60);
                        
                        
                        console.log({ minuteDifference })
                        
                        if (minuteDifference > 30) {
                            hasError = true;
                            error = "This code has expired. Please request a new one."
                        }
                    }
                }

                if (field.name === "password") {
                    setPasswordRequirementState(field.value)
                } else {
                    if (errorSection) {
                        errorSection.innerHTML = hasError ? error : "";
                    }
                }
                
                if (hasError) {
                    console.log({ field })
                    fieldsWithError += 1;
                    field.classList.add("with-error")
                } else { 
                    sectionData = {
                        ...sectionData,
                        [field.name]: field.value
                    }
                    field.classList.remove("with-error") 
                }
            })
           // console.log({ fieldsWithError: fieldsWithError });
            
            
            if (sectionKey === "subscription") {
                let selectedMembership = $(".membership-list .membership-item.selected");
                console.log(selectedMembership);
                let errorElement = section.querySelector(".error-text");
                if (selectedMembership.length === 0) {
                    //console.log("No membership selected");
                    errorElement.innerHTML = "You need to select a membership"
                    fieldsWithError += 1;
                } else {
                    let selection = selectedMembership[0]
                    console.log("Selected:", selection);
           
                    let membershipName = selection.dataset.item;
                    let membershipPrice = selection.dataset.price;
                    let membershipJoiningFee = selection.dataset.fee
                    let membershipId = selection.dataset.id;
                    console.log({ membershipName, membershipPrice, membershipJoiningFee, membershipId });
                    
                    savedUserData.membership = {
                        id: parseInt(membershipId),
                        name: membershipName,
                        price: parseFloat(membershipPrice),
                        joiningFee: parseFloat(membershipJoiningFee),
                    }
                    
                   // console.log({ savedUserData  });
                    errorElement.innerHTML = ""
                }
            } else {
                savedUserData[sectionKey] = sectionData;
            }
            
            if (fieldsWithError === 0) {
                
               console.log({ savedUserData });
                let nextSection = $("#memberForm .form-section")[buttonIndex + 1];
                
                if (nextSection) {
                    let key = nextSection.dataset.section;
                    console.log(nextSection);
                    if (key === "accountVerification") {
                        sendEmail(sectionData.email)
                            .then(response => {
                                console.log(response);
                                verificationCode = response.body.value;
                            }).catch(error => {
                                console.log(error);
                        })
                    } else if (key === "review"){
                        let membership = savedUserData.membership;
                        let totalPrice = membership.price + membership.joiningFee
                        
                        if (savedUserData.discount && "percentage" in savedUserData.discount) {
                            let discountValue = totalPrice * savedUserData.discount.percentage;
                            console.log(discountValue);
                            totalPrice = (totalPrice - discountValue).toFixed(2);
                        }
                        
                        let body = nextSection.querySelector(".section-body");
                        
                        
                        if (body) {
                            body.innerHTML = `<div class="review-body text-center">
                                <div class="review-title">
                                    You are purchasing a <span class="fw-bold">${ savedUserData.membership.name }</span> membership
                                </div>
                                <div class="review-price">
                                    <span class="review-currency">&#163;</span>
                                    <span class="price-tag">${ totalPrice }</span>
                                </div>
                                
                                <div class="review-breakdown">${ '&#163;' + membership.price + ' monthly price' }${ membership.joiningFee ? ` + ${ '&#163;' + membership.joiningFee  + ' joining fee '}`: ""}${ savedUserData.discount ? ` + ${ savedUserData.discount['percentage'] * 100 }% discount`: ""}</div>
                                
                                <button class="button button-primary account-btn">Create account</button>
                     
                            </div>`;
                        }
                        
                    }
                }
                
                moveToStep(buttonIndex + 1);
                savedStep = buttonIndex + 1;
            }
        }
        
        
    }
})


$(document).on("click", ".account-btn", function (e) {
    e.preventDefault();
    console.log("Account:", savedUserData);


   /*let data = {
        "personalInfo": {
        "firstName": "John",
            "lastName": "Doe",
            "email": "kuteyisamueldev@gmail.com",
            "gender": "Male",
            "phone": "+4407444104253",
            "password": "Sam987412!",
            "confirmPassword": "Sam987412!"
    },
        "accountVerification": {
        "verificationCode": "212307"
    },
        "discount": {
        "id": 2,
            "code": "JOIN15",
            "percentage": 0.15,
            "createdAt": "2024-10-31T06:45:19",
            "expirationDate": "2025-04-30T06:45:19"
    },
        "membership": {
        "id": 3,
            "name": "Key Access",
            "price": 45,
            "joiningFee": 70
    }
    }*/
    
    let { firstName, lastName, email, gender, phone, password } = savedUserData['personalInfo']
    let requestData = {
        firstName,
        lastName,
        email,
        gender,
        phone,
        password,
        membership: savedUserData.membership.id,
        discount: savedUserData.discount ? savedUserData.discount.id : null,
    }
    
    console.log({ requestData })
    
    $(this).text("");
    $(this).append(`<div class="spinner"></div>`);
    
    const resetAccountButton = () => {
        setTimeout(() => {
            $(this).removeClass("button-error")
            $(this).removeClass("button-success");
            $(this).text("Create account")
        }, 3000)
    }
    
    const setErrorState = (errorText = '') => {
        console.log("This", $(this))
        $(this).addClass("button-error")
        $(this).text(errorText ?? "Error creating account");
        $(".account-btn .spinner").remove();

        resetAccountButton()
    }
    
    createUser(requestData, "Member").then((response) => {
        console.log("Response:", response)
        if (response.statusCode === 200) {
            $(this).addClass("button-success");
            $(this).text("Account created successfully.");
    
            resetAccountButton();
        } else {
            
            setErrorState(response.body?.message ?? "");
        }
    }).catch(error => {
        console.log("Error:", error);
      
        setErrorState()
    })
})