"use strict";

document.getElementById("displayname").value = prompt("Enter your Name:");

var connection = new signalR.HubConnectionBuilder().withUrl("/ChatHub").build();

connection.on("BroadcastMessage", function (name, message) {
    var msg = message.replace(/&/g, "&amp;").replace(/</g, "&lt;").replace(/>/g, "&gt;");
    var encodedMsg = name + ": " + msg;
    var li = document.createElement("li");
    li.textContent = encodedMsg;
    document.getElementById("discussion").appendChild(li);
});

connection.start().catch(function (err) {
    return console.error(err.toString());
});

document.getElementById("sendmessage").addEventListener("click", function (event) {
    var user = document.getElementById("displayname").value;
    var message = document.getElementById("message").value;
    connection.invoke("Send", user, message).catch(function (err) {
        return console.error(err.toString());
    });
    event.preventDefault();
});