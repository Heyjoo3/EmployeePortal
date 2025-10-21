export function canEditStatus(
  currentEmployeeId,
  relationship,
  status,
  id,
  startDate,
  employeeStatus,
  substituteStatus,
  supervisorStatus,
  adminStatus,
  substitute = null
) {
  const isAdmin = relationship === 'admin'
  const isFutureDate = new Date(startDate) >= new Date().setHours(0, 0, 0, 0)
  const isCurrentEmployee = id === currentEmployeeId

  // Definiert spezifische Statusbedingungen in benannten Funktionen
  const isLeaveApprovedUndocumented = status === 1 && adminStatus === 3 && isAdmin && id !== null // null is for company vacations
  const isLeaveApproved = status === 1 && employeeStatus === 6 && !isAdmin
  const isLeaveApprovedAdminIsSubstitute =
    status === 1 && adminStatus === 1 && isAdmin && id !== null

  const isLeaveRejectedBySubstitute =
    status === 2 && substituteStatus === 2 && relationship === 'substitute'
  const isLeaveRejectedBySupervisor =
    status === 2 && supervisorStatus === 2 && relationship === 'supervisor'
  const isLeaveRejectedAdminSubstitute =
    status === 2 && substituteStatus === 2 && isAdmin && currentEmployeeId === substitute

  const isLeavePendingBySubstitute =
    status === 3 &&
    substituteStatus === 3 &&
    supervisorStatus === 3 &&
    relationship === 'substitute'
  const isLeavePendingBySupervisor =
    status === 3 &&
    substituteStatus === 1 &&
    supervisorStatus === 3 &&
    relationship === 'supervisor' // Substitut hat Urlaub genehmigt, Vorgesetzter hat Urlaub offen
  const isLeavePendingAdminSubstitute =
    status === 3 &&
    substituteStatus === 3 &&
    supervisorStatus === 3 &&
    isAdmin &&
    currentEmployeeId === substitute

  const isCancellationApprovedUndocumented =
    status === 4 && adminStatus === 6 && isAdmin && id !== null // null is for company vacations

  const isCancellationRejectedBySubstitute =
    status === 5 && substituteStatus === 5 && relationship === 'substitute'
  const isCancellationRejectedBySupervisor =
    status === 5 && supervisorStatus === 5 && relationship === 'supervisor'
  const isCancellationRejectedAdminSubstitute =
    status === 5 && substituteStatus === 5 && isAdmin && currentEmployeeId === substitute
  const isCancellationRejectedByEmployee =
    status === 5 && employeeStatus === 5 && relationship === 'employee'

  const isCancellationPendingNonAdmin = status === 6 && relationship !== 'admin'
  const isCancellationPendingAdminSubstitute =
    status === 6 && isAdmin && currentEmployeeId === substitute

  // Kombiniere die Bedingungen zu einer einzigen `editStatus`-Bedingung
  const editStatus =
    (isCurrentEmployee || isAdmin) &&
    isFutureDate &&
    (isLeaveApprovedUndocumented ||
      isLeaveApproved ||
      isLeaveApprovedAdminIsSubstitute ||
      isLeaveRejectedBySubstitute ||
      isLeaveRejectedBySupervisor ||
      isLeaveRejectedAdminSubstitute ||
      isLeavePendingBySubstitute ||
      isLeavePendingBySupervisor ||
      isLeavePendingAdminSubstitute ||
      isCancellationApprovedUndocumented ||
      isCancellationRejectedBySubstitute ||
      isCancellationRejectedBySupervisor ||
      isCancellationRejectedAdminSubstitute ||
      isCancellationRejectedByEmployee ||
      isCancellationPendingNonAdmin ||
      isCancellationPendingAdminSubstitute)

  return editStatus
}
