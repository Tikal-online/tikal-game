import { Pipe, PipeTransform } from '@angular/core';

@Pipe({
  name: 'enumMap',
})
export class EnumMapPipe<T> implements PipeTransform {
  transform(value: unknown, enumMap: object, defaultValue?: T): T {
    return enumMap[value as keyof typeof enumMap] ?? defaultValue ?? value;
  }
}
