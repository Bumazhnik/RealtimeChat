import { IPublicUserDTO } from "./dto/DTO";
import { getJson } from "./requests";

export let myUser: IPublicUserDTO;
export async function loadMyUser() {
    myUser = await getJson(`/account/myuser`);
    console.log(myUser);
}
