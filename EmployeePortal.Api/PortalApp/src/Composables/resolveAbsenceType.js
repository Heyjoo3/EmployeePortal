export function resolveAbsenceType() {
  const absenceTypes = [
    'Krankheit',
    'Unfall',
    'Weiterbildung',
    'Elternzeit',
    'Kinderbetreuung',
    'Mutterschutz',
    'Andere'
  ]

  const absenceTypeMap = {
    1: 'Andere',
    2: 'Krankheit',
    3: 'Unfall',
    4: 'Weiterbildung',
    5: 'Elternzeit',
    6: 'Kinderbetreuung',
    7: 'Mutterschutz',
    Andere: 1,
    Krankheit: 2,
    Unfall: 3,
    Weiterbildung: 4,
    Elternzeit: 5,
    Kinderbetreuung: 6,
    Mutterschutz: 7
  }

  function enumToString(enumValue) {
    return absenceTypeMap[enumValue] || 'Andere'
  }

  function stringToEnum(stringValue) {
    return absenceTypeMap[stringValue] || 1
  }

  return {
    absenceTypes,
    enumToString,
    stringToEnum
  }
}
