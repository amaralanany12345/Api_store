import { Order } from "./order";

export interface Receipt {
    id:number,
    createdAt:Date,
    totalAmount:number,
    order:Order,
    orderId:number,
}
