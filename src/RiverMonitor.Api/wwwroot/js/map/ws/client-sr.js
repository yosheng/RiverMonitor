// SignalR Initialization
const connection = new signalR.HubConnectionBuilder()
    .withUrl("/chatHub")
    .withAutomaticReconnect()
    .build();

connection.start()
    .then(() => console.log("SignalR connected"))
    .catch(err => console.error("SignalR connection error:", err));

connection.on("ReceiveMessage", function (user, message) {
    console.log(`[SignalR] ${user}: ${message}`);
});

