export interface IMessageDTO {
    userName: string;
    chatSessionId: number;
    text: string;
    date: string;
}
export interface IPublicUserDTO {
    id: number;
    name: string;
    avatarUrl: string;
}
export interface IChatSessionDTO {
    id: number;
    ownerId: number;
    name: string;
    users: IPublicUserDTO[];
}
