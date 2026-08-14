import { Category } from "./category";
import { OrderItem } from "./order-item";

export interface Item {
    id:number,
    name:string,
    price:number,
    stockQuantity:number,
    category:Category,
    categoryId:number,
    orderItems:OrderItem[]
}
