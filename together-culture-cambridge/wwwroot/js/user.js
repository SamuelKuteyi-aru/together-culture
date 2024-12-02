var systemUser;
const loadUser = async () => {
    let response = await fetch("/EndUser", {
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
            window.location.href = "/EndUser/Login";
        }
        throw new Error(response.statusText);
    }

    return await response.json();
    
}
$(document).ready(function () {
    $(".breadcrumb .breadcrumb-item.sub").text(getBreadcrumbPage());
    let loadSection = $(".layout-load-section.process-section");
    let errorSection = $(".layout-load-section.error-section");
    let mainSection = $("main.main-container");
    
    const createEventGrid = () => {
       let container = $(".data-grid.event-grid");
       createEventDataList(container, systemUser.eventBookings, "UserDashboard");

        $(".event-section .no-data-list").text( systemUser.eventBookings.length === 0 ? "You have not booked any spaces at any events" : "");
    }

    const createSpaceGrid = () => {
        let container = $(".data-grid.space-grid");
        createSpaceDataList(container, systemUser.spaceBookings, "UserDashboard");

        $(".space-section .no-data-list").text(systemUser.spaceBookings.length === 0 ? "You do not currently have any seats booked at any of our spaces" : "");
    }
    loadUser().then(user => {
        systemUser = user;
        loadSection.remove();
        errorSection.remove();
 
        let userName = user.user.firstName + " " + user.user.lastName?.slice(0, 1);
        $(".main-header .user-section .user-name").text(userName);
        mainSection.removeClass("d-none");

        let path = window.location.pathname.slice(1).split(/\//g);
        if (path.length >= 2 && path[1] === 'Dashboard') {
            console.log({ user })
            
            createEventGrid()
            createSpaceGrid();
            
            
        }
    }).catch(error => {
        loadSection.remove();
        errorSection.removeClass("d-none");
        $(".layout-load-section.error-section .load-text").text(error.message);
    })
    
    const setButtonErrorState = (button) => {
        button.removeClass("button-secondary");
        button.addClass("button-error");
        button.text("Error cancelling booking")

        setTimeout(function () {
            button.removeClass("button-error");
            button.removeClass("button-success");

            button.addClass("button-secondary");
            button.text("Cancel booking");

            button.removeAttr("disabled")
        }, 3000)
    }
    
    $(document).on("click", ".event-grid .event-cancel-btn", function () {
        let index = $(".event-grid .event-cancel-btn").index(this);
        
        $(this).text("Cancelling booking..."); 
        $(this).attr("disabled", "disabled");
        
        let event = systemUser.eventBookings[index];
        cancelEventBooking(event.id).then(() => {
            systemUser.eventBookings = systemUser.eventBookings.filter(booking => booking.id !== event.id);
            createEventGrid();
        }).catch(err => {
            setButtonErrorState($(this))
        })
        
    })

    $(document).on("click", ".space-grid .space-cancel-btn", function () {
        let index = $(".space-grid .space-cancel-btn").index(this);
        console.log({ index })
        $(this).text("Cancelling booking...");
        $(this).attr("disabled", "disabled");
        
        let space = systemUser.spaceBookings[index];
        cancelSpaceBooking(space.id).then(() => {
            systemUser.spaceBookings = systemUser.eventBookings.filter(booking => booking.id !== space.id);
            createSpaceGrid()
        }).catch(() => {
            setButtonErrorState($(this))
        })
    })
})