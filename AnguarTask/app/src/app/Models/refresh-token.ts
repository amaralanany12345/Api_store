import { User } from "./user";

export interface RefreshToken {
    id:number,
    token:string,
    userId:number,
    user:User,
    createdAt:Date,
    expiredAt:Date,
    // isValid => DateTime.Now < ExpiredAt;
}
