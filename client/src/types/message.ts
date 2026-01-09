export type Message = {
    id: string
    senderId: string
    senderDisplayName: string
    senderImageUrl: string
    recipientId: string
    reciepientDisplayName: string
    recipientImageUrl: string
    content: string
    dateRead?: string
    messageSent: string
    currentUserSender?:boolean
}