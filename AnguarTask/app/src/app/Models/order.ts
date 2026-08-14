import { OrderItem } from "./order-item"
import { Receipt } from "./receipt"
import { User } from "./user"

export interface Order {
    id:number,
    createdAt:Date,
    updatedAt:Date,
    status:string,
    totalAmount:number,
    customer:User,
    customerId:number,
    orderItems:OrderItem[],
    receipt:Receipt
}
