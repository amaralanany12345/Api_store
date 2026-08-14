import { Item } from "./item";
import { Order } from "./order";

export interface OrderItem {
    orderId:number,
    order:Order,
    itemId:number,
    item:Item,
    quantity:number,
}
