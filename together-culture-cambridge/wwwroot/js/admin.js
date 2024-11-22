
const loadAdmin = async () => {
    let response = await fetch("/Admin", {
        method: "GET",
        headers: {
            Accept: 'application/json',
            'Content-Type': 'application/json',
        },
        credentials: 'include',
    })
    
    if (!response.ok) {
        console.log({ response })
        if (response.status === 401) {
            window.location.href = "/Admin/Login";
        }
        throw new Error(response.statusText);
    }
    
    let body = await response.json();
    
    return body.value;
}

const approveAccount = async (userId) => {
    let response = await fetch(`/Admin/Approve/${userId}`, {
        method: "POST",
        headers: {
            Accept: 'application/json',
            'Content-Type': 'application/json',
        },
        credentials: "include",
    })
    
    if (!response.ok) {
        console.log("Error response:", response);
        throw new Error(response.statusText);
    }
    
    return await response.json();
}
const loadUnapprovedAccounts = async () => {
    let response = await fetch(`/EndUser/Unapproved`, {
        method: "GET",
        headers: {
            Accept: 'application/json',
            'Content-Type': 'application/json',
        },
        credentials: 'include',
    })

    if (!response.ok) {
        console.log({ response })
        throw new Error(response.statusText);
    }
    
    
    let body = await response.json();
    return body;
}

var systemAdmin, usersList = [];

const createUserList = () => {
    let userGrid = $(".user-grid");
    userGrid.html("");
    usersList.forEach(user => {
       
        let date= new Date(user.createdAt).toDateString();
        userGrid.append(`
        <div class="grid-item">
            <div class="user-img-icon">
                <img alt="user" src="/icons/user-icon.svg"/>
            </div>
            
            <div class="user-name my-2">${user.firstName } ${ user.lastName }</div>
            <div class="user-email">${ user.email }</div>
            <div class="user-phone mt-1">${ user.phone }</div>
            
            <div class="user-membership mt-3">${ user.membership.name } Membership</div>
            <div class="application-date mt-1">Applied on ${ date }</div>
            
            <button class="button button-secondary approve-btn mt-3">
                Approve
            </button>
        </div>
        `)
    })
}

const getUnapprovedAccounts = () => {
    let userErrorSection = $(".user-list-section .load-error-section");
    let userLoadSection = $(".user-list-section .loading-section");
    let userGrid = $(".user-list-section .user-grid");

    userErrorSection.addClass("d-none");
    userGrid.addClass("d-none");
    
    console.log("Fetching unapproved accounts")

    userLoadSection.removeClass("d-none");
    loadUnapprovedAccounts().then(list => {
        userLoadSection.remove();
        userErrorSection.remove();
        
        
        userGrid.removeClass("d-none");
        usersList = [ ...list ];
        
        
        if (list.length === 0) {
            addEmptyListSection()
        }
        console.log("Response:", list);
        createUserList();
    }).catch(err => {
        console.log("Load error:", err)
        userErrorSection.removeClass("d-none");
        userLoadSection.addClass("d-none");



        $(".user-list-section .load-error-section .spinner + span").text(err.message);
    })
}

const addEmptyListSection = () => {
        $(".user-list-section .empty-list-section").remove();
       
        let grid = $(".user-list-section .user-grid")
        if (!grid.hasClass("d-none")) {
           grid.addClass("d-none");
        }
        
        $(".user-list-section").append(
            `<div class='text-center py-5'>
                There are currently no unapproved users.
            </div>`
        )
}
$(document).ready(function () {
    let href = window.location.href;
    let hrefSplit = href.split('/');
    let lastSplitValue = hrefSplit[hrefSplit.length - 1];
    $(".breadcrumb .breadcrumb-item.sub").text(lastSplitValue);
    
    let loadSection = $(".layout-load-section.process-section");
    let errorSection = $(".layout-load-section.error-section");
    let mainSection = $("main.main-container");
    
    loadAdmin()
        .then(response => {
            console.log("Response: ", response);
            loadSection.remove();
            errorSection.remove();
            
            
            systemAdmin = response;
            let userName = systemAdmin.firstName + " " + systemAdmin.lastName?.slice(0, 1);
            $(".main-header .user-section .user-name").text(userName);
            $(".header-section .user-welcome .user-name").text(systemAdmin.firstName);
            mainSection.removeClass("d-none");
            
            
           
            getUnapprovedAccounts();
        }).catch(error => {
            console.log("Error: ", error);
            loadSection.remove();
            errorSection.removeClass("d-none")
            $(".layout-load-section.error-section .load-text").text(error.message);
        })
    
    
    $(document).on("click", ".grid-item .approve-btn", function (event) {
          let index = $(".grid-item .approve-btn").index(this);
          console.log("Index:", index);
          
          let user = usersList[index];
          
          $(this).attr("disabled", "disabled");
          $(this).text("Approving account...")
          console.log({ user })
        
        approveAccount(user.id)
            .then(response => {
                console.log("Response:", response);
                $(this).removeClass("button-secondary");
                $(this).addClass("button-success");
                $(this).text("Account approved!");
                
                usersList = usersList.filter(item => item.id !== user.id);
                
                
                setTimeout(() => {
                    let gridItem = $(".user-grid .grid-item");
                    if (gridItem[index]) {
                        gridItem.addClass("transition");
                        setTimeout(() => {
                            gridItem.remove();
                            if (usersList.length === 0) {
                                addEmptyListSection()
                            }
                        }, 250)
                    }
                }, 3000)
            }).catch(err => {
                console.log("Error: ", err);
                $(this).addClass("button-error");
                $(this).text("Error approving account");
                $(this).removeAttr("disabled");
                
                setTimeout(() => {
                    $(this).removeClass("button-error");
                    $(this).text("Approve");
                }, 3000)
            })
    })
})