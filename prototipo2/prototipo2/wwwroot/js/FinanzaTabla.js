// Inicializar DataTable para Movimientos Financieros
document.addEventListener('DOMContentLoaded', function() {
    const datatablesSimple = document.getElementById('datatablesFinanza');
    if (datatablesSimple)
    {
        new simpleDatatables.DataTable(datatablesSimple, {
            perPage: 10, // 10 filas por página
            labels: {
        placeholder: "Buscar movimiento...", // Texto del buscador
                noRows: "No se encontraron movimientos", // Mensaje sin datos
                info: "Mostrando {start} a {end} de {rows} movimientos", // Texto de paginación
            },
            columns:
        [
                { select: 3, sort: "desc" }, // Ordenar por Fecha (columna 4) descendente al inicio
                { select: 4, type: "date", format: "DD/MM/YYYY" }, // Formato para fecha de vencimiento
                { select: 1, type: "number" } // Tratar el monto como número
            ]
        });
}
});