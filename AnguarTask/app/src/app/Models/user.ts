import { Order } from "./order"
import { RefreshToken } from "./refresh-token"

export interface User {
    id:number
    userName:string
    email:string
    passwordHash:string
    role:string
    createdAt:Date
    orders:Order[]
    refreshTokens:RefreshToken[]
}
