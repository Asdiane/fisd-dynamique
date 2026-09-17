export enum ArticleStatus {
  Published = 'published',
  Draft = 'draft',
}

export enum ContentStatus {
  Published = 'published',
  Draft = 'draft',
}

export interface Article {
  id: string;
  title: string;
  slug: string | null;
  excerpt: string | null;
  content: string;
  imageUrl: string | null;
  status: ArticleStatus;
  publishedAt: string | null;
  createdAt: string;
  updatedAt: string;
}

export enum SouvenirStatus {
  Published = 'published',
  ComingSoon = 'coming_soon',
}

export interface Souvenir {
  id: string;
  year: number;
  title: string;
  description: string | null;
  status: SouvenirStatus;
}

export interface SouvenirPhoto {
  id: string;
  souvenirId: string;
  imageUrl: string;
  caption: string | null;
  displayOrder: number;
}

// Public-facing shape - already resolved to the visitor's language by the API.
export interface Testimonial {
  imageUrl?: string | null;
  id: string;
  authorName: string;
  authorRole: string | null;
  content: string;
  displayOrder: number;
}

// Admin-facing shape - carries both languages so the edit form can show them side by side.
export interface AdminTestimonial {
  imageUrl?: string | null;
  id: string;
  authorName: string;
  authorRoleFr: string | null;
  authorRoleEn: string | null;
  contentFr: string;
  contentEn: string;
  isVisible: boolean;
  status: ContentStatus;
  displayOrder: number;
}

export enum ContactType {
  Phone = 'phone',
  WhatsApp = 'whatsapp',
  Email = 'email',
  Address = 'address',
  Social = 'social',
}

export interface Contact {
  id: string;
  type: ContactType;
  label: string;
  value: string;
  href: string | null;
  isVisible: boolean;
  status: ContentStatus;
  displayOrder: number;
}

export enum AdminRole {
  Editor = 'Editor',
  SuperAdmin = 'SuperAdmin',
  PlatformAdmin = 'PlatformAdmin',
}

export interface AdminUser {
  id: string;
  email: string;
  role: AdminRole;
  createdAt: string;
  isActive: boolean;
  twoFactorEnabled: boolean;
}

// Public-facing shape - already resolved to the visitor's language by the API.
export interface Edition {
  id: string;
  year: number;
  badge: string;
  title: string;
  text: string;
  startDate: string | null;
  endDate: string | null;
  locationLabel: string | null;
  ticketingUrl: string | null;
  isCurrent: boolean;
}

// Admin-facing shape - carries both languages so the edit form can show them side by side.
export interface AdminEdition {
  id: string;
  year: number;
  badgeFr: string;
  badgeEn: string;
  titleFr: string;
  titleEn: string;
  textFr: string;
  textEn: string;
  startDate: string | null;
  endDate: string | null;
  locationLabelFr: string | null;
  locationLabelEn: string | null;
  ticketingUrl: string | null;
  isVisible: boolean;
  status: ContentStatus;
  isCurrent: boolean;
}

export interface ProgramDay {
  id: string;
  editionId: string;
  label: string;
  dateLabel: string;
  displayOrder: number;
}

export interface ScheduleItem {
  id: string;
  programDayId: string;
  time: string;
  title: string;
  tag: string;
  detail: string;
  location: string;
  displayOrder: number;
}

// Public-facing shape - already resolved to the visitor's language by the API.
export interface Slide {
  id: string;
  imageUrl: string;
  videoUrl: string | null;
  focalPoint: string | null;
  kicker: string;
  title: string;
  text: string;
  place: string | null;
  displayOrder: number;
}

// Admin-facing shape - carries both languages so the edit form can show them side by side.
export interface AdminSlide {
  id: string;
  imageUrl: string;
  videoUrl: string | null;
  focalPoint: string | null;
  kickerFr: string;
  kickerEn: string;
  titleFr: string;
  titleEn: string;
  textFr: string;
  textEn: string;
  placeFr: string | null;
  placeEn: string | null;
  isVisible: boolean;
  status: ContentStatus;
  displayOrder: number;
}

// Public-facing shape - already resolved to the visitor's language by the API.
export interface Pillar {
  id: string;
  title: string;
  text: string;
  imageUrl: string;
  icon: string;
  displayOrder: number;
}

// Admin-facing shape - carries both languages so the edit form can show them side by side.
export interface AdminPillar {
  id: string;
  titleFr: string;
  titleEn: string;
  textFr: string;
  textEn: string;
  imageUrl: string;
  icon: string;
  isVisible: boolean;
  status: ContentStatus;
  displayOrder: number;
}

// Public-facing shape - already resolved to the visitor's language by the API.
export interface Participant {
  id: string;
  name: string;
  description: string;
  displayOrder: number;
}

// Admin-facing shape - carries both languages so the edit form can show them side by side.
export interface AdminParticipant {
  id: string;
  nameFr: string;
  nameEn: string;
  descriptionFr: string;
  descriptionEn: string;
  isVisible: boolean;
  status: ContentStatus;
  displayOrder: number;
}

// Public-facing shape - already resolved to the visitor's language by the API.
export interface Speaker {
  id: string;
  name: string;
  role: string;
  imageUrl: string;
  description: string;
  displayOrder: number;
}

// Admin-facing shape - carries both languages so the edit form can show them side by side.
export interface AdminSpeaker {
  id: string;
  editionId: string;
  name: string;
  roleFr: string;
  roleEn: string;
  imageUrl: string;
  descriptionFr: string;
  descriptionEn: string;
  isVisible: boolean;
  status: ContentStatus;
  displayOrder: number;
}

export interface Partner {
  id: string;
  name: string;
  label: string | null;
  logoUrl: string;
  isVisible: boolean;
  status: ContentStatus;
  displayOrder: number;
}

// Public-facing shape - already resolved to the visitor's language by the API.
export interface EngagementAction {
  id: string;
  anchor: string;
  title: string;
  text: string;
  icon: string;
  imageUrl: string;
  link: string;
  cta: string;
  detail: string;
  displayOrder: number;
}

// Admin-facing shape - carries both languages so the edit form can show them side by side.
export interface AdminEngagementAction {
  id: string;
  anchor: string;
  titleFr: string;
  titleEn: string;
  textFr: string;
  textEn: string;
  icon: string;
  imageUrl: string;
  link: string;
  ctaFr: string;
  ctaEn: string;
  detailFr: string;
  detailEn: string;
  isVisible: boolean;
  status: ContentStatus;
  displayOrder: number;
}

// Reader-facing shape - already resolved to the reader's language by the API.
export interface HelpArticle {
  id: string;
  category: string;
  title: string;
  content: string;
  displayOrder: number;
}

// PlatformAdmin-facing shape - carries both languages so the management form can show them side by side.
export interface AdminHelpArticle {
  id: string;
  category: string;
  titleFr: string;
  titleEn: string;
  contentFr: string;
  contentEn: string;
  isVisible: boolean;
  displayOrder: number;
  isPlatformAdminOnly: boolean;
}

export enum TicketCategory {
  Bug = 'bug',
  Improvement = 'improvement',
  Other = 'other',
}

export enum TicketStatus {
  Open = 'open',
  InProgress = 'in_progress',
  Resolved = 'resolved',
  Rejected = 'rejected',
}

export interface TicketAttachment {
  id: string;
  originalFileName: string;
  contentType: string;
  fileSizeBytes: number;
  url: string;
  createdAt: string;
}

export interface Ticket {
  id: string;
  category: TicketCategory;
  title: string;
  description: string;
  status: TicketStatus;
  createdByEmail: string;
  createdAt: string;
  resolutionNote: string | null;
  resolvedAt: string | null;
  attachments: TicketAttachment[];
}
