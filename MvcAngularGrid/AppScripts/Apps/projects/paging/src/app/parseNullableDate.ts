export function parseNullableDate(input: string): Date | null {
    let r: Date | null = null;
    if (input) {
        r = new Date(input);
    }

    return r;
}
