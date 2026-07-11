import { ABP, ApplicationConfigurationDto, ConfigStateService } from '@abp/ng.core';
import { Injectable } from '@angular/core';
import { map } from 'rxjs/operators';

@Injectable({ providedIn: 'root' })
export class RoleService {
    constructor(protected configState: ConfigStateService) {}

    getGrantedRole$(key: string) {
        return this.getStream().pipe(map((grantedRoles) => this.isInRole(key, grantedRoles)));
    }

    getGrantedRole(key: string | undefined) {
        const roles = this.getSnapshot();
        return this.isInRole(key, roles);
    }

    // filterItemsByPolicy<T extends ABP.HasPolicy>(items: Array<T>) {
    //     const policies = this.getSnapshot();
    //     return items.filter((item) => !item.requiredPolicy || this.isInRole(item.requiredPolicy, policies));
    // }

    // filterItemsByPolicy$<T extends ABP.HasPolicy>(items: Array<T>) {
    //     return this.getStream().pipe(
    //         map((policies) =>
    //             items.filter((item) => !item.requiredPolicy || this.isInRole(item.requiredPolicy, policies))
    //         )
    //     );
    // }

    protected isInRole(key: string | undefined, roles: string[]) {
        if (!key) return true;

        const orRegexp = /\|\|/g;
        const andRegexp = /&&/g;

        // NOTE: Allow combination of ANDs & ORs
        if (orRegexp.test(key)) {
            const keys = key.split('||').filter(Boolean);

            if (keys.length < 2) return false;

            return keys.some((k) => this.getRole(k.trim(), roles));
        } else if (andRegexp.test(key)) {
            const keys = key.split('&&').filter(Boolean);

            if (keys.length < 2) return false;

            return keys.every((k) => this.getRole(k.trim(), roles));
        }

        return this.getRole(key, roles);
    }

    protected getStream() {
        return this.configState.getAll$().pipe(map(this.mapToRoles));
    }

    protected getSnapshot() {
        return this.mapToRoles(this.configState.getAll());
    }

    protected mapToRoles(applicationConfiguration: ApplicationConfigurationDto) {
        return applicationConfiguration?.currentUser?.roles || [];
    }

    protected getRole(key: string, roles: string[]) {
        return roles.some((role) => role.toLowerCase() === key.toLowerCase());
    }
}
