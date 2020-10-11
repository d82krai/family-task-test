var _taskId;

window.getTaskId = () => {
    return _taskId;
};

document.addEventListener("dragenter", function (event) {
    // highlight potential drop target when the draggable element enters it
    if (event.target.className.indexOf("menu-item") > -1) {
        event.target.setAttribute("taskId", _taskId);
    }

}, false);

document.addEventListener("dragleave", function (event) {
    // reset background of potential drop target when the draggable element leaves it
    if (event.target.className.indexOf("menu-item") > -1) {
        event.target.setAttribute("taskId", "");
    }

}, false);


function allowDrop(ev) {
    ev.preventDefault();
}

function dragEnter(ev) {
    debugger 
    if (ev.target.className == "menu-item") {
        ev.target.style.background = "purple";
    }
}

function drag(ev) {
    ev.dataTransfer.setData("taskId", ev.target.getAttribute("taskId"));
    _taskId = ev.target.getAttribute("taskId");
}

function drop(ev) {
    debugger
    ev.preventDefault();
    var taskId = ev.dataTransfer.getData("taskId");
    var memberId = ev.target.getAttribute("memberId");
    UpdateAssignedTo(taskId, memberId);
}

function UpdateAssignedTo(taskId, memberId) {
    var params = JSON.stringify({ taskId: taskId, memberId: memberId });
    var saveData = $.ajax({
        type: 'POST',
        url: "/api/Tasks/UpdateAssignedTo",
        data: params,
        dataType: "text",
        success: function (resultData) { alert("Save Complete") }
    });
    saveData.error(function () { alert("Something went wrong"); })
}