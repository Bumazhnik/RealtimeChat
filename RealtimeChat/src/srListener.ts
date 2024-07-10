import * as signalR from "@microsoft/signalr";
import { IMessageDTO } from "./dto/DTO";
import { elements } from "./elements";
import { myUser } from "./myUser";
import { globalState } from "./state";
console.log("signalr initiated");
const connection = new signalR.HubConnectionBuilder()
    .withUrl("/chatHub")
    .build();

connection.on("MessageReceived", (message: IMessageDTO) => {
    if (message.chatSessionId == globalState.currentSessionId)
        elements.chatContent.appendChild(
            elements.createMessageElement(
                message,
                myUser.name == message.userName
            )
        );
});

connection.start().catch((err) => console.error(err));
