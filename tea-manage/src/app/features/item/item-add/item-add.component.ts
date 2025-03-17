import { CommonModule } from '@angular/common';
import { Component, EventEmitter, inject, Output } from '@angular/core';
import {
  FormArray,
  FormBuilder,
  FormGroup,
  FormsModule,
  ReactiveFormsModule,
  Validators,
} from '@angular/forms';
import { NzButtonModule } from 'ng-zorro-antd/button';
import { NzFormModule } from 'ng-zorro-antd/form';
import { NzInputModule } from 'ng-zorro-antd/input';
import { NzModalModule } from 'ng-zorro-antd/modal';
import { ItemService } from '../../../core/services/item.service';
import { ToastrService } from 'ngx-toastr';
import { UtilitiesService } from '../../../core/services/utilities.service';
import { NzCheckboxModule } from 'ng-zorro-antd/checkbox';
import { CategoryService } from '../../../core/services/category.service';
import { Category } from '../../../shared/models/category';
import { NzSelectModule } from 'ng-zorro-antd/select';
import { QuillModule } from 'ngx-quill';
import { NzImageModule } from 'ng-zorro-antd/image';
import { Router } from '@angular/router';

@Component({
  selector: 'app-item-add',
  standalone: true,
  imports: [
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
  ],
  templateUrl: './item-add.component.html',
  styleUrl: './item-add.component.css',
})
export class ItemAddComponent {
  private itemService = inject(ItemService);
  private toastrService = inject(ToastrService);
  private utilService = inject(UtilitiesService);
  private categoryService = inject(CategoryService);
  private router = inject(Router);
  private fb = inject(FormBuilder);
  frm: FormGroup = new FormGroup({});
  validationErrors?: string[];
  categories: Category[] = [];
  imgPreview: string | ArrayBuffer | null = null;

  ngOnInit(): void {
    this.loadCategory();
    this.initForm();
  }

  initForm() {
    this.frm = this.fb.group({
      name: ['', Validators.required],
      slug: ['', Validators.required],
      description: [''],
      featured: [false],
      isPublished: [false],
      isFeatured: [false],
      categoryIdList: [[], Validators.required],
      sizes: this.fb.array([], Validators.required),
      imgFile: ['', Validators.required],
    });

    this.frm.get('name')?.valueChanges.subscribe((name) => {
      const slug = this.utilService.generateSlug(name);
      this.frm.get('slug')?.setValue(slug, { emitEvent: false }); // Cập nhật trường slug
    });
  }

  get sizes() {
    return this.frm.get('sizes') as FormArray;
  }

  submitForm() {
    const formData = new FormData();
    formData.append('name', this.frm.value.name);
    formData.append('description', this.frm.value.description);
    formData.append('slug', this.frm.value.slug);
    formData.append('isPublished', this.frm.value.isPublished);
    formData.append('isFeatured', this.frm.value.isFeatured);

    const imageFile = this.frm.get('imgFile')?.value;
    formData.append('imgFile', imageFile);

    const categoryIdList = this.frm.value.categoryIdList;
    categoryIdList.forEach((id: number) => {
      formData.append('categoryIdList', id.toString());
    });

    const sizes = this.frm.value.sizes;
    const sizeCreateRequestJson = JSON.stringify(sizes);
    formData.append('SizeCreateRequestJson', sizeCreateRequestJson);

    this.itemService.add(formData).subscribe({
      next: (res) => {
        this.toastrService.success('Thêm sản phẩm thành công');
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

  addSize() {
    const sizeGroup = this.fb.group({
      name: ['', Validators.required],
      price: ['', Validators.required],
      newPrice: [''],
      description: [''],
    });
    this.sizes.push(sizeGroup);
  }

  removeSize(index: number) {
    this.sizes.removeAt(index);
  }

  onFileSelected(event: Event) {
    const input = event.target as HTMLInputElement;
    if (input.files && input.files[0]) {
      const file = input.files[0];
      const reader = new FileReader();
      reader.onload = () => {
        this.imgPreview = reader.result;
      };
      reader.readAsDataURL(file);
      this.frm.get('imgFile')?.setValue(file);
    }
  }

  handleCancel() {
    this.frm.reset();
    this.categories = [];
    this.imgPreview = null;
    this.validationErrors = [];
    this.router.navigateByUrl('/san-pham');
  }
}
