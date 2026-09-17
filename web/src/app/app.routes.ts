import { Routes } from '@angular/router';
import { authGuard, superAdminGuard, platformAdminGuard } from './core/auth.guard';
import { unsavedChangesGuard } from './core/unsaved-changes.guard';

export const routes: Routes = [
  { path: '', loadComponent: () => import('./pages/home/home').then((m) => m.HomeComponent) },
  {
    path: 'agir',
    loadComponent: () => import('./pages/engagement/engagement').then((m) => m.EngagementComponent),
  },
  {
    path: 'programmation',
    loadComponent: () =>
      import('./pages/programming/programming').then((m) => m.ProgrammingComponent),
  },
  {
    path: 'programmation/:edition',
    loadComponent: () =>
      import('./pages/programming/programming').then((m) => m.ProgrammingComponent),
  },
  {
    path: 'rapport-synthese',
    loadComponent: () => import('./pages/report/report').then((m) => m.ReportComponent),
  },
  {
    path: 'galerie',
    loadComponent: () => import('./pages/gallery/gallery').then((m) => m.GalleryComponent),
  },
  {
    path: 'actualites',
    loadComponent: () => import('./pages/news/news').then((m) => m.NewsComponent),
  },
  {
    path: 'actualites/:id',
    loadComponent: () =>
      import('./pages/news-detail/news-detail').then((m) => m.NewsDetailComponent),
  },
  {
    path: 'contact',
    loadComponent: () => import('./pages/contact/contact').then((m) => m.ContactComponent),
  },
  {
    path: 'reservation',
    loadComponent: () =>
      import('./pages/reservation/reservation').then((m) => m.ReservationComponent),
  },
  {
    path: 'conditions',
    loadComponent: () => import('./pages/conditions/conditions').then((m) => m.ConditionsComponent),
  },
  {
    path: 'admin',
    children: [
      {
        path: '',
        loadComponent: () =>
          import('./pages/admin/admin-login/admin-login').then((m) => m.AdminLoginComponent),
      },
      {
        path: '2fa-setup',
        loadComponent: () =>
          import('./pages/admin/admin-2fa-setup/admin-2fa-setup').then(
            (m) => m.Admin2faSetupComponent,
          ),
      },
      {
        path: '2fa-verify',
        loadComponent: () =>
          import('./pages/admin/admin-2fa-verify/admin-2fa-verify').then(
            (m) => m.Admin2faVerifyComponent,
          ),
      },
      {
        path: 'forgot-password',
        loadComponent: () =>
          import('./pages/admin/admin-forgot-password/admin-forgot-password').then(
            (m) => m.AdminForgotPasswordComponent,
          ),
      },
      {
        path: 'reset-password',
        loadComponent: () =>
          import('./pages/admin/admin-reset-password/admin-reset-password').then(
            (m) => m.AdminResetPasswordComponent,
          ),
      },
      {
        path: 'accepter-invitation',
        loadComponent: () =>
          import('./pages/admin/admin-accept-invitation/admin-accept-invitation').then(
            (m) => m.AdminAcceptInvitationComponent,
          ),
      },
      {
        path: '',
        canActivate: [authGuard],
        loadComponent: () =>
          import('./pages/admin/admin-shell/admin-shell').then((m) => m.AdminShellComponent),
        children: [
          {
            path: 'pages',
            canDeactivate: [unsavedChangesGuard],
            loadComponent: () =>
              import('./pages/admin/admin-editorial/admin-editorial').then(
                (m) => m.AdminEditorialComponent,
              ),
          },
          {
            path: 'articles',
            loadComponent: () =>
              import('./pages/admin/admin-articles/admin-articles').then(
                (m) => m.AdminArticlesComponent,
              ),
          },
          {
            path: 'articles/new',
            canDeactivate: [unsavedChangesGuard],
            loadComponent: () =>
              import('./pages/admin/admin-article-form/admin-article-form').then(
                (m) => m.AdminArticleFormComponent,
              ),
          },
          {
            path: 'articles/:id',
            canDeactivate: [unsavedChangesGuard],
            loadComponent: () =>
              import('./pages/admin/admin-article-form/admin-article-form').then(
                (m) => m.AdminArticleFormComponent,
              ),
          },
          {
            path: 'contacts',
            loadComponent: () =>
              import('./pages/admin/admin-contacts/admin-contacts').then(
                (m) => m.AdminContactsComponent,
              ),
          },
          {
            path: 'contacts/new',
            loadComponent: () =>
              import('./pages/admin/admin-contact-form/admin-contact-form').then(
                (m) => m.AdminContactFormComponent,
              ),
          },
          {
            path: 'contacts/:id',
            loadComponent: () =>
              import('./pages/admin/admin-contact-form/admin-contact-form').then(
                (m) => m.AdminContactFormComponent,
              ),
          },
          {
            path: 'temoignages',
            loadComponent: () =>
              import('./pages/admin/admin-testimonials/admin-testimonials').then(
                (m) => m.AdminTestimonialsComponent,
              ),
          },
          {
            path: 'temoignages/new',
            loadComponent: () =>
              import('./pages/admin/admin-testimonial-form/admin-testimonial-form').then(
                (m) => m.AdminTestimonialFormComponent,
              ),
          },
          {
            path: 'temoignages/:id',
            loadComponent: () =>
              import('./pages/admin/admin-testimonial-form/admin-testimonial-form').then(
                (m) => m.AdminTestimonialFormComponent,
              ),
          },
          {
            path: 'souvenirs',
            loadComponent: () =>
              import('./pages/admin/admin-souvenirs/admin-souvenirs').then(
                (m) => m.AdminSouvenirsComponent,
              ),
          },
          {
            path: 'souvenirs/new',
            loadComponent: () =>
              import('./pages/admin/admin-souvenir-form/admin-souvenir-form').then(
                (m) => m.AdminSouvenirFormComponent,
              ),
          },
          {
            path: 'souvenirs/:id/edit',
            loadComponent: () =>
              import('./pages/admin/admin-souvenir-form/admin-souvenir-form').then(
                (m) => m.AdminSouvenirFormComponent,
              ),
          },
          {
            path: 'souvenirs/:id',
            loadComponent: () =>
              import('./pages/admin/admin-souvenir-detail/admin-souvenir-detail').then(
                (m) => m.AdminSouvenirDetailComponent,
              ),
          },
          {
            path: 'editions',
            loadComponent: () =>
              import('./pages/admin/admin-editions/admin-editions').then(
                (m) => m.AdminEditionsComponent,
              ),
          },
          {
            path: 'editions/new',
            loadComponent: () =>
              import('./pages/admin/admin-edition-form/admin-edition-form').then(
                (m) => m.AdminEditionFormComponent,
              ),
          },
          {
            path: 'editions/:id/edit',
            loadComponent: () =>
              import('./pages/admin/admin-edition-form/admin-edition-form').then(
                (m) => m.AdminEditionFormComponent,
              ),
          },
          {
            path: 'editions/:id',
            loadComponent: () =>
              import('./pages/admin/admin-edition-detail/admin-edition-detail').then(
                (m) => m.AdminEditionDetailComponent,
              ),
          },
          {
            path: 'diaporama',
            loadComponent: () =>
              import('./pages/admin/admin-slides/admin-slides').then((m) => m.AdminSlidesComponent),
          },
          {
            path: 'diaporama/new',
            loadComponent: () =>
              import('./pages/admin/admin-slide-form/admin-slide-form').then(
                (m) => m.AdminSlideFormComponent,
              ),
          },
          {
            path: 'diaporama/:id',
            loadComponent: () =>
              import('./pages/admin/admin-slide-form/admin-slide-form').then(
                (m) => m.AdminSlideFormComponent,
              ),
          },
          {
            path: 'piliers',
            loadComponent: () =>
              import('./pages/admin/admin-pillars/admin-pillars').then(
                (m) => m.AdminPillarsComponent,
              ),
          },
          {
            path: 'piliers/new',
            loadComponent: () =>
              import('./pages/admin/admin-pillar-form/admin-pillar-form').then(
                (m) => m.AdminPillarFormComponent,
              ),
          },
          {
            path: 'piliers/:id',
            loadComponent: () =>
              import('./pages/admin/admin-pillar-form/admin-pillar-form').then(
                (m) => m.AdminPillarFormComponent,
              ),
          },
          {
            path: 'participants',
            loadComponent: () =>
              import('./pages/admin/admin-participants/admin-participants').then(
                (m) => m.AdminParticipantsComponent,
              ),
          },
          {
            path: 'participants/new',
            loadComponent: () =>
              import('./pages/admin/admin-participant-form/admin-participant-form').then(
                (m) => m.AdminParticipantFormComponent,
              ),
          },
          {
            path: 'participants/:id',
            loadComponent: () =>
              import('./pages/admin/admin-participant-form/admin-participant-form').then(
                (m) => m.AdminParticipantFormComponent,
              ),
          },
          {
            path: 'intervenants',
            loadComponent: () =>
              import('./pages/admin/admin-speakers/admin-speakers').then(
                (m) => m.AdminSpeakersComponent,
              ),
          },
          {
            path: 'intervenants/new',
            loadComponent: () =>
              import('./pages/admin/admin-speaker-form/admin-speaker-form').then(
                (m) => m.AdminSpeakerFormComponent,
              ),
          },
          {
            path: 'intervenants/:id',
            loadComponent: () =>
              import('./pages/admin/admin-speaker-form/admin-speaker-form').then(
                (m) => m.AdminSpeakerFormComponent,
              ),
          },
          {
            path: 'partenaires',
            loadComponent: () =>
              import('./pages/admin/admin-partners/admin-partners').then(
                (m) => m.AdminPartnersComponent,
              ),
          },
          {
            path: 'partenaires/new',
            loadComponent: () =>
              import('./pages/admin/admin-partner-form/admin-partner-form').then(
                (m) => m.AdminPartnerFormComponent,
              ),
          },
          {
            path: 'partenaires/:id',
            loadComponent: () =>
              import('./pages/admin/admin-partner-form/admin-partner-form').then(
                (m) => m.AdminPartnerFormComponent,
              ),
          },
          {
            path: 'engagement',
            loadComponent: () =>
              import('./pages/admin/admin-engagement-actions/admin-engagement-actions').then(
                (m) => m.AdminEngagementActionsComponent,
              ),
          },
          {
            path: 'engagement/new',
            loadComponent: () =>
              import('./pages/admin/admin-engagement-action-form/admin-engagement-action-form').then(
                (m) => m.AdminEngagementActionFormComponent,
              ),
          },
          {
            path: 'engagement/:id',
            loadComponent: () =>
              import('./pages/admin/admin-engagement-action-form/admin-engagement-action-form').then(
                (m) => m.AdminEngagementActionFormComponent,
              ),
          },
          {
            path: 'media',
            loadComponent: () =>
              import('./pages/admin/admin-media/admin-media').then((m) => m.AdminMediaComponent),
          },
          {
            path: 'securite',
            loadComponent: () =>
              import('./pages/admin/admin-security/admin-security').then(
                (m) => m.AdminSecurityComponent,
              ),
          },
          {
            path: 'users',
            canActivate: [superAdminGuard],
            loadComponent: () =>
              import('./pages/admin/admin-users/admin-users').then((m) => m.AdminUsersComponent),
          },
          {
            path: 'users/new',
            canActivate: [superAdminGuard],
            loadComponent: () =>
              import('./pages/admin/admin-user-form/admin-user-form').then(
                (m) => m.AdminUserFormComponent,
              ),
          },
          {
            path: 'journal-erreurs',
            canActivate: [platformAdminGuard],
            loadComponent: () =>
              import('./pages/admin/admin-error-logs/admin-error-logs').then(
                (m) => m.AdminErrorLogsComponent,
              ),
          },
          {
            path: 'journal-audit',
            canActivate: [platformAdminGuard],
            loadComponent: () =>
              import('./pages/admin/admin-audit-logs/admin-audit-logs').then(
                (m) => m.AdminAuditLogsComponent,
              ),
          },
          {
            path: 'sante-systeme',
            canActivate: [platformAdminGuard],
            loadComponent: () =>
              import('./pages/admin/admin-system-health/admin-system-health').then(
                (m) => m.AdminSystemHealthComponent,
              ),
          },
          {
            path: 'email-history',
            canActivate: [platformAdminGuard],
            loadComponent: () =>
              import('./pages/admin/admin-email-history/admin-email-history').then(
                (m) => m.AdminEmailHistoryComponent,
              ),
          },
          {
            path: 'help',
            loadComponent: () =>
              import('./pages/admin/admin-help/admin-help').then((m) => m.AdminHelpComponent),
          },
          {
            path: 'help/manage',
            canActivate: [platformAdminGuard],
            loadComponent: () =>
              import('./pages/admin/admin-help-articles/admin-help-articles').then(
                (m) => m.AdminHelpArticlesComponent,
              ),
          },
          {
            path: 'help/manage/new',
            canActivate: [platformAdminGuard],
            loadComponent: () =>
              import('./pages/admin/admin-help-article-form/admin-help-article-form').then(
                (m) => m.AdminHelpArticleFormComponent,
              ),
          },
          {
            path: 'help/manage/:id',
            canActivate: [platformAdminGuard],
            loadComponent: () =>
              import('./pages/admin/admin-help-article-form/admin-help-article-form').then(
                (m) => m.AdminHelpArticleFormComponent,
              ),
          },
          {
            path: 'tickets',
            loadComponent: () =>
              import('./pages/admin/admin-tickets/admin-tickets').then(
                (m) => m.AdminTicketsComponent,
              ),
          },
          {
            path: 'tickets/manage',
            canActivate: [platformAdminGuard],
            loadComponent: () =>
              import('./pages/admin/admin-tickets-manage/admin-tickets-manage').then(
                (m) => m.AdminTicketsManageComponent,
              ),
          },
        ],
      },
    ],
  },
  { path: '**', redirectTo: '' },
];
