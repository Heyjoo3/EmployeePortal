export function useDateConverter() {
  function localizedDate(date) {
    const options = {
      year: 'numeric',
      month: 'short',
      day: 'numeric'
    }
    return new Date(date).toLocaleDateString('de-DE', options)
  }

  function getWeekday( dweekdayindex) {
    const weekdays = {
      0: 'Sonntag',
      1: 'Montag',
      2: 'Dienstag',
      3: 'Mittwoch',
      4: 'Donnerstag',
      5: 'Freitag',
      6: 'Samstag'
    }
    return weekdays[dweekdayindex]
  }

  function convertToGermanISO(dateString) {
    // Parse the date string
    const date = new Date(dateString)

    // Get the UTC time of the date
    const utcDate = new Date(date.getTime() - date.getTimezoneOffset() * 60000)

    // Determine if DST is in effect
    const isDST =
      date.getTimezoneOffset() <
      Math.max(
        new Date(date.getFullYear(), 0, 1).getTimezoneOffset(),
        new Date(date.getFullYear(), 6, 1).getTimezoneOffset()
      )

    // Adjust the ISO string for CET or CEST
    const timezoneOffset = isDST ? '+02:00' : '+01:00'

    // Return the ISO string with the correct timezone offset
    return utcDate.toISOString().replace('Z', timezoneOffset)
  }

  //day.month.year --> IsoString
  function convertGermanDateToISO(dateString) {
    // split the date string into day, month and year
    const [day, month, year] = dateString.split('.')
    // Parse the date string
    const date = new Date(year, month - 1, day)

    // Get the UTC time of the date
    const utcDate = new Date(date.getTime() - date.getTimezoneOffset() * 60000)

    // Determine if DST is in effect
    const isDST =
      date.getTimezoneOffset() <
      Math.max(
        new Date(date.getFullYear(), 0, 1).getTimezoneOffset(),
        new Date(date.getFullYear(), 6, 1).getTimezoneOffset()
      )

    // Adjust the ISO string for CET or CEST
    const timezoneOffset = isDST ? '+02:00' : '+01:00'

    // Return the ISO string with the correct timezone offset
    return utcDate.toISOString().replace('Z', timezoneOffset)
  }

  function convertToISO(dateString) {
    const months = {
      Januar: 0,
      Februar: 1,
      März: 2,
      April: 3,
      Mai: 4,
      Juni: 5,
      Juli: 6,
      August: 7,
      September: 8,
      Oktober: 9,
      November: 10,
      Dezember: 11
    }

    dateString = dateString.replace('.', '')
    const [day, monthName, year] = dateString.split(' ')
    const month = months[monthName]
    return new Date(year, month, parseInt(day)).toISOString()
  }

  return {
    convertToGermanISO,
    convertGermanDateToISO,
    localizedDate,
    convertToISO,
    getWeekday
  }
}
