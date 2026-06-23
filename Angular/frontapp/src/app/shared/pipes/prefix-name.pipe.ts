import { Pipe, PipeTransform } from '@angular/core';

@Pipe({
  name: 'prefixName',
  pure: true,
})
export class PrefixNamePipe implements PipeTransform {
  transform(value: string) {
    return this.extractPrefix(value);
  }

  // IMPROVEMENT: reemplazar los doble espacios por un solo espacio.
  extractPrefix(persona: string): string {
    // Dividir la cadena en partes separadas por espacios
    const partes = persona.split(' ');

    // Asegurarnos de que la cadena tiene al menos tres partes
    if (partes.length < 3) {
      return '...';
    }

    // Extraer el primer apellido (primer carácter), el segundo apellido y el primer nombre
    const primerApellido = partes[0].trim().charAt(0);
    const segundoApellido = partes[1].trim().charAt(0);
    const primerNombre = partes[2].trim().charAt(0);

    // Devolver los tres elementos concatenados
    return `${primerApellido}${segundoApellido}${primerNombre}`.toLocaleUpperCase();
  }
}
