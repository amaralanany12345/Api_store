import { Item } from "./item";

export interface Category {
    id:string,
    name:string,
    description:string,
    items:Item[]
}
