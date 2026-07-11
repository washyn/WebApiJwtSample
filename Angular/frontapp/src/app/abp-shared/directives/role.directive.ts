import { PermissionService, QUEUE_MANAGER, QueueManager } from '@abp/ng.core';
import {
  AfterViewInit,
  ChangeDetectorRef,
  Directive,
  Inject,
  Input,
  OnChanges,
  OnDestroy,
  Optional,
  TemplateRef,
  ViewContainerRef,
} from '@angular/core';
import { ReplaySubject, Subscription } from 'rxjs';
import { distinctUntilChanged, take } from 'rxjs/operators';
import { RoleService } from '../services/role.service';

@Directive({
  selector: '[appRole]',
  standalone: true,
})
export class RoleDirective implements OnDestroy, OnChanges, AfterViewInit {
  @Input('appRole') condition: string | undefined;

  @Input('appRoleRunChangeDetection') runChangeDetection = true;

  subscription!: Subscription;

  cdrSubject = new ReplaySubject<void>();

  rendered = false;

  constructor(
    @Optional() private templateRef: TemplateRef<any>,
    private vcRef: ViewContainerRef,
    private roleService: RoleService,
    private cdRef: ChangeDetectorRef,
    @Inject(QUEUE_MANAGER) public queue: QueueManager
  ) {}

  private check() {
    if (this.subscription) {
      this.subscription.unsubscribe();
    }

    this.subscription = this.roleService
      .getGrantedRole$(this.condition || '')
      .pipe(distinctUntilChanged())
      .subscribe((isGranted) => {
        this.vcRef.clear();
        if (isGranted) this.vcRef.createEmbeddedView(this.templateRef);
        if (this.runChangeDetection) {
          if (!this.rendered) {
            this.cdrSubject.next();
          } else {
            this.cdRef.detectChanges();
          }
        } else {
          this.cdRef.markForCheck();
        }
      });
  }

  ngOnDestroy(): void {
    if (this.subscription) this.subscription.unsubscribe();
  }

  ngOnChanges() {
    this.check();
  }

  ngAfterViewInit() {
    this.cdrSubject.pipe(take(1)).subscribe(() => this.queue.add(() => this.cdRef.detectChanges()));
    this.rendered = true;
  }
}
