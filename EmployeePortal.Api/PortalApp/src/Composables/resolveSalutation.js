export function resolveSalutation() {
  const salutations = ['Herr', 'Frau', 'Divers']

  const salutationMap = {
    1: 'Herr',
    2: 'Frau',
    3: 'Divers',
    Herr: 1,
    Frau: 2,
    Divers: 3
  }

  function enumToString(enumValue) {
    return salutationMap[enumValue] || 'Andere'
  }

  function stringToEnum(stringValue) {
    return salutationMap[stringValue] || 3
  }

  return {
    salutations,
    enumToString,
    stringToEnum
  }
}
