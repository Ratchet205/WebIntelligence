let chatCounter = 0;
const IDbegin = "ChatID-";

function createChat() {
    let ChatCount = chatCounter++;
    let chat = document.createElement("button");

    chat.setAttribute("id", IDbegin+ChatCount);
    chat.setAttribute("class", "chatButton");
    chat.textContent = "Chat "+ChatCount;
    chat.setAttribute("style", 'z-index:99, position:absolute');


    let navbar = document.getElementById("container-left");

    navbar.appendChild(chat);
}