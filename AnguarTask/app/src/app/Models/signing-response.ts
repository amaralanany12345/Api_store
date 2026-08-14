import { RefreshToken } from "./refresh-token";
import { User } from "./user";

export interface SigningResponse {
    user:User,
    jwtToken:string,
    refreshToken:RefreshToken,
}
