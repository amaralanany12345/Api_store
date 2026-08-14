import { Routes } from '@angular/router';
import { SigninComponent } from './Components/signin-component/signin-component';
import { SignUpComponent } from './Components/sign-up-component/sign-up-component';
import { HomeComponent } from './Components/home-component/home-component';
import { ItemDetails } from './Components/item-details/item-details';
import { AllOrders } from './Components/all-orders/all-orders';
import { UserOrder } from './Components/user-order/user-order';
import { OrderDetails } from './Components/order-details/order-details';

export const routes: Routes = [
    {path:'',component:SigninComponent},
    {path:'signUp',component:SignUpComponent},
    {path:'home',component:HomeComponent},
    {path:'items/:id',component:ItemDetails},
    {path:'orders',component:AllOrders},
    {path:'orders/:id',component:OrderDetails},
];
