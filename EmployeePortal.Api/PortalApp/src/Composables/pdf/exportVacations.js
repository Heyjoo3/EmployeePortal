import { resolveVacationStatus } from '../resolveVacationStatus'

export function exportVacations(vacationData, username) {
  const printWindow = window.open('', '', 'height=600,width=800')

  printWindow.document.write('<html><head><title>Urlaubsübersicht</title>')
  printWindow.document.write(`
    <style>
      * {
    margin: 0;
    padding: 0;
    box-sizing: border-box;
      }
    
      @media print {
        header, footer {
          display: block;
          width: 100%;
          background-color: #09599c;
          color: white;
          text-align: center;
          padding: 10px 0;
        }
        body { margin: 10; }
        footer { position: fixed; bottom: 0; width: 100%; }
      }

      /* Default styles */
      body { font-family: Arial, sans-serif; }
      h1 { text-align: center; }
      table { width: 100%; border-collapse: collapse; margin-top: 20px; }
      th, td { border: 1px solid #ddd; padding: 8px; text-align: left; }
      th { background-color: #f2f2f2; }
      tr:nth-child(even) { background-color: #f9f9f9; }
    </style>
  `)
  printWindow.document.write('</head>')

  // Header and Footer
  printWindow.document.write('<header style="background-color: #09599c; height: 50px;"></header>')
  printWindow.document.write(
    '<footer style="background-color: #09599c; height: 50px; position: fixed; bottom: 0; width: 100%;"></footer>'
  )
  printWindow.document.write('<body>')
  printWindow.document.write('<h1>Urlaubsübersicht</h1>')
  printWindow.document.write(
    `<h2 style="display: inline-block; font-size: 22px;">${username}</h2><h2 style="display: inline-block; float: right; font-size: 22px;">Datum: ${new Date().toLocaleDateString()}</h2>`
  )
  printWindow.document.write('<p style="font-size: 12px;">Ein * steht für einen Halben Tag</p>')
  printWindow.document.write('<table>')
  printWindow.document.write(`
    <tr>
      <th>Startdatum</th>
      <th>Enddatum</th>
      <th>Dauer</th>
      <th>Typ</th>
      <th>Status</th>
      <th>Anmerkung</th>
    </tr>
  `)

  // Table Content
  vacationData.forEach((vacation) => {
    const formatDate = (date) => {
      const d = new Date(date)
      const day = String(d.getDate()).padStart(2, '0')
      const month = String(d.getMonth() + 1).padStart(2, '0')
      const year = d.getFullYear()
      return `${day}.${month}.${year}`
    }

    const startDate = formatDate(vacation.startDate) + (vacation.isHalfStartDay ? ' *' : '')
    const endDate = formatDate(vacation.endDate) + (vacation.isHalfEndDay ? ' *' : '')

    printWindow.document.write(`
      <tr>
        <td>${startDate}</td>
        <td>${endDate}</td>
        <td>${vacation.vacationdays} Tage</td>
        <td>${vacation.type}</td>
        <td>${resolveVacationStatus().enumToString(vacation.status)}</td>
        <td>${vacation.description ? vacation.description : ''}</td>
      </tr>
    `)
  })

  printWindow.document.write('</table>')
  printWindow.document.write('</body></html>')

  printWindow.document.close()

  printWindow.onload = function () {
    printWindow.print()
  }
}
