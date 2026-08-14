import { Component, OnInit, signal } from '@angular/core';
import { ItemService } from '../../Services/item-service';
import { ItemDto } from '../../Dtos/item-dto';
import { ActivatedRoute, Router } from '@angular/router';

@Component({
  selector: 'app-item-details',
  imports: [],
  templateUrl: './item-details.html',
  styleUrl: './item-details.css',
})
export class ItemDetails implements OnInit {

  item=signal<ItemDto| null>(null)
  constructor(private itemService:ItemService,private activaedRouter:ActivatedRoute,private router:Router){}
  ngOnInit(): void {
    const itemId=Number(this.activaedRouter.snapshot.paramMap.get('id'))
    this.itemService.GetItemById(itemId).subscribe({
      next:(res)=>{
        this.item.set(res)
      }
    })
  }

}
