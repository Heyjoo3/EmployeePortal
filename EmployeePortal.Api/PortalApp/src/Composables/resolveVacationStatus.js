export function resolveVacationStatus() {
  const vacationStatuses = [
    'Nicht Relevant',
    'Urlaub Genehmigt',
    'Urlaub Abgelehnt',
    'Urlaub Ausstehend',
    'Stornierung Genehmigt',
    'Stornierung Abgelehnt',
    'Stornierung Ausstehend'
  ]

  const vacationStatusMap = {
    0: 'Nicht Relevant',
    1: 'Urlaub Genehmigt',
    2: 'Urlaub Abgelehnt',
    3: 'Urlaub Ausstehend',
    4: 'Stornierung Genehmigt',
    5: 'Stornierung Abgelehnt',
    6: 'Stornierung Ausstehend',
    'Nicht Relevant': 0,
    'Urlaub Genehmigt': 1,
    'Urlaub Abgelehnt': 2,
    'Urlaub Ausstehend': 3,
    'Stornierung Genehmigt': 4,
    'Stornierung Abgelehnt': 5,
    'Stornierung Ausstehend': 6
  }

  function enumToString(enumValue) {
    return vacationStatusMap[enumValue] || 'Andere'
  }

  function stringToEnum(stringValue) {
    return vacationStatusMap[stringValue] || 0
  }

  return {
    vacationStatuses,
    enumToString,
    stringToEnum
  }
}
