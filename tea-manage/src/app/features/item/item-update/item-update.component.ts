import { CommonModule } from '@angular/common';
import { Component, inject, OnInit } from '@angular/core';
import {
  FormArray,
  FormBuilder,
  FormGroup,
  FormsModule,
  ReactiveFormsModule,
  Validators,
} from '@angular/forms';
import { NzButtonModule } from 'ng-zorro-antd/button';
import { NzCheckboxModule } from 'ng-zorro-antd/checkbox';
import { NzFormModule } from 'ng-zorro-antd/form';
import { NzImageModule } from 'ng-zorro-antd/image';
import { NzInputModule } from 'ng-zorro-antd/input';
import { NzModalModule, NzModalService } from 'ng-zorro-antd/modal';
import { NzSelectModule } from 'ng-zorro-antd/select';
import { NzTabsModule } from 'ng-zorro-antd/tabs';
import { QuillModule } from 'ngx-quill';
import { ItemService } from '../../../core/services/item.service';
import { ToastrService } from 'ngx-toastr';
import { UtilitiesService } from '../../../core/services/utilities.service';
import { CategoryService } from '../../../core/services/category.service';
import { Category } from '../../../shared/models/category';
import { ActivatedRoute, Router } from '@angular/router';
import { Item, ItemUpdate } from '../../../shared/models/item';
import { Size, SizeAdd, SizeUpdate } from '../../../shared/models/size';
import { NzDividerModule } from 'ng-zorro-antd/divider';

@Component({
  selector: 'app-item-update',
  standalone: true,
  imports: [
    NzTabsModule,
    NzButtonModule,
    NzModalModule,
    NzFormModule,
    ReactiveFormsModule,
    NzInputModule,
    CommonModule,
    NzCheckboxModule,
    NzSelectModule,
    FormsModule,
    QuillModule,
    NzImageModule,
    NzDividerModule,
    NzModalModule,
  ],
  templateUrl: './item-update.component.html',
  styleUrl: './item-update.component.css',
})
export class ItemUpdateComponent implements OnInit {
  private itemService = inject(ItemService);
  private toastrService = inject(ToastrService);
  private utilService = inject(UtilitiesService);
  private categoryService = inject(CategoryService);
  private activatedRoute = inject(ActivatedRoute);
  private router = inject(Router);
  private fb = inject(FormBuilder);
  private modal = inject(NzModalService);
  frm: FormGroup = new FormGroup({});
  frmSize: FormGroup = new FormGroup({});
  validationErrors?: string[];
  validationAddSizeErrors?: string[];
  validationUpdateSizeErrors?: string[];
  validationUpdateImageErrors?: string[];
  categories: Category[] = [];
  sizes: Size[] = [];
  item?: Item;

  ngOnInit(): void {
    this.loadCategory();
    this.loadItem();
    this.initForm();
    this.initSizeForm();
  }

  loadItem() {
    const id = this.activatedRoute.snapshot.paramMap.get('id');
    if (!id) return;
    this.itemService.get(+id).subscribe({
      next: (res) => {
        const item = res as Item;
        this.patchForm(item);
        this.pathSizesValue(item.sizes);
        this.item = item;
      },
      error: (er) => console.log(er),
    });
  }

  patchForm(item: Item) {
    console.log(item);
    const categoryIds = item.categories?.map((category) => category.id);
    this.frm.patchValue({
      id: item.id,
      name: item.name,
      slug: item.slug,
      description: item.description,
      isFeatured: item.isFeatured,
      isPublished: item.isPublished,
      categoryIdList: categoryIds,
    });
  }

  initForm() {
    this.frm = this.fb.group({
      name: ['', Validators.required],
      slug: ['', Validators.required],
      description: [''],
      isFeatured: [false],
      isPublished: [false],
      categoryIdList: [[], Validators.required],
    });

    this.frm.get('name')?.valueChanges.subscribe((name) => {
      const slug = this.utilService.generateSlug(name);
      this.frm.get('slug')?.setValue(slug, { emitEvent: false }); // Cập nhật trường slug
    });
  }

  submitItemForm() {
    console.log(this.frm.value.categoryIdList);
    const itemUpdate: ItemUpdate = {
      id: this.item?.id ?? 0,
      name: this.frm.value.name,
      description: this.frm.value.description,
      slug: this.frm.value.slug,
      isPublished: this.frm.value.isPublished,
      isFeatured: this.frm.value.isFeatured,
      categoryIdList: this.frm.value.categoryIdList ?? [],
    };

    this.itemService.update(itemUpdate).subscribe({
      next: (res) => {
        this.toastrService.success('Cập nhật sản phẩm thành công');
        this.item = res as Item;
        this.handleCancel();
      },
      error: (er) => {
        this.validationErrors = er;
        console.log(er);
      },
    });
  }

  loadCategory() {
    this.categoryService.getAll().subscribe({
      next: (res) => {
        this.categories = res;
      },
      error: (er) => {
        console.log(er);
      },
    });
  }

  resetPage() {
    this.categories = [];
    this.sizes = [];
  }

  resetError() {
    this.validationErrors = [];
    this.validationAddSizeErrors = [];
    this.validationUpdateImageErrors = [];
    this.validationUpdateSizeErrors = [];
  }

  resetForm() {
    this.frm.reset();
    this.frmSize.reset();
  }

  handleCancel() {
    this.resetError();
    this.resetPage();
    this.resetForm();
    this.router.navigateByUrl('/san-pham');
  }

  initSizeForm() {
    this.frmSize = this.fb.group({
      name: ['', Validators.required],
      price: ['', Validators.required],
      newPrice: [null],
      description: [''],
    });
  }

  pathSizesValue(sizes: Size[]) {
    this.sizes = sizes;
  }

  submitSizeForm() {
    const sizeAdd: SizeAdd = {
      name: this.frmSize.value.name,
      price: this.frmSize.value.price,
      newPrice: this.frmSize.value.newPrice ?? null,
      description: this.frmSize.value.description,
    };
    const sizes: SizeAdd[] = [];
    sizes.push(sizeAdd);
    this.itemService.addSizes(this.item?.id ?? 0, sizes).subscribe({
      next: (res) => {
        this.item = res as Item;
        this.sizes = this.item.sizes;
        this.toastrService.success('Thêm size cho sản phẩm thành công');
        this.resetError();
      },
      error: (er) => {
        this.validationAddSizeErrors = er;
        console.log(er);
      },
    });
  }

  removeSize(id: number) {
    const idList: number[] = [];
    idList.push(id);
    this.itemService.deleteSizes(this.item?.id ?? 0, idList).subscribe({
      next: (_) => {
        this.toastrService.success('Xoá size của sản phẩm thành công');
        const index = this.sizes.findIndex((x) => x.id == id);
        if (index !== -1) {
          this.sizes.splice(index, 1);
        } else {
          this.toastrService.error('Vui lòng tải lại trang');
        }
      },
      error: (er) => {
        console.log(er);
      },
    });
  }

  showDeleteConfirm(id: number): void {
    this.modal.confirm({
      nzTitle: 'Bạn có chắc muốn xoá dòng này?',
      nzContent:
        '<b style="color: red;">Sau khi xoá sẽ không thể hoàn tác lại.</b>',
      nzOkText: 'Xác nhận',
      nzOkType: 'primary',
      nzOkDanger: true,
      nzOnOk: () => {
        this.removeSize(id);
      },
      nzCancelText: 'Huỷ',
      nzOnCancel: () => console.log('Cancel'),
    });
  }

  updateSize(size: Size) {
    const sizeUpdate: SizeUpdate = {
      id: size.id,
      name: size.name,
      description: size.description,
      price: size.price,
      newPrice: size.newPrice,
      itemId: this.item?.id ?? 0,
    };

    this.itemService.updateSize(sizeUpdate).subscribe({
      next: (res) => {
        this.item = res as Item;
        this.sizes = this.item.sizes;
        this.toastrService.success('Cập nhật size thành công');
        this.resetError();
      },
      error: (er) => {
        this.validationUpdateSizeErrors = er;
        console.log(er);
      },
    });
  }

  onFileSelected(event: Event) {
    const input = event.target as HTMLInputElement;
    if (input.files && input.files[0]) {
      const file = input.files[0];
      const formData = new FormData();
      formData.append('imgFile', file);
      this.itemService.updateImage(this.item?.id ?? 0, formData).subscribe({
        next: (res) => {
          this.toastrService.success('Cập nhật hình ảnh thành công');
          const response = res as any;
          if (this.item) this.item.imgUrl = response.imgUrl;
        },
        error: (er) => {
          this.validationUpdateImageErrors = er;
          console.log(er);
        },
      });
    }
  }
}
