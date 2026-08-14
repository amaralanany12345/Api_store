import { Component, OnInit, signal } from '@angular/core';
import { OrderDto } from '../../Dtos/order-dto';
import { OrderService } from '../../Services/order-service';
import { OrderItemDto } from '../../Dtos/order-item-dto';
import { ActivatedRoute } from '@angular/router';

@Component({
  selector: 'app-order-details',
  imports: [],
  templateUrl: './order-details.html',
  styleUrl: './order-details.css',
})
export class OrderDetails implements OnInit {


  order=signal<OrderDto|null>(null)
  orderCartItems=signal<OrderItemDto[]>([])
  constructor(private orderService:OrderService,private activatedRoute:ActivatedRoute){}
  ngOnInit(): void {
    const orderId=Number(this.activatedRoute.snapshot.paramMap.get('id'))
    this.orderService.GetOrderItemsById(orderId).subscribe({
      next:(res)=>{
        this.orderCartItems.set(res)
      }
    })
  }

}
