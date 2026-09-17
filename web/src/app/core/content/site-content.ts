export const ZEFFY_URL =
  'https://www.zeffy.com/fr-CA/ticketing/forum-international-solidarite-et-developpement-fisd--2026';

export interface Slide {
  image: string;
  // CSS object-position for the background image - defaults to 'center'. Close-up
  // portrait photos need the crop biased toward the top so the subject's face stays in frame
  // once object-cover stretches a 3:2 photo to fill the much taller hero banner.
  focalPoint?: string;
  kicker: string;
  title: string;
  text: string;
  place?: string;
}

export interface Pillar {
  title: string;
  text: string;
  image: string;
  icon: string;
}

export interface Participant {
  name: string;
  description: string;
}

export interface Speaker {
  name: string;
  role: string;
  image: string;
  description: string;
}

export interface Partner {
  name: string;
  label?: string;
  logo: string;
}

export interface EngagementAction {
  anchor: string;
  title: string;
  text: string;
  icon: string;
  image: string;
  link: string;
  cta: string;
  detail: string;
}

export interface ScheduleItem {
  time: string;
  title: string;
  tag: string;
  detail: string;
  location: string;
}

export interface ProgramDay {
  id: string;
  label: string;
  date: string;
  schedule: ScheduleItem[];
}

export interface Program {
  id: string;
  badge: string;
  title: string;
  text: string;
  days: ProgramDay[];
}

export interface StaticPhoto {
  caption: string;
  imageUrl: string;
  description: string;
}

export interface StaticEvent {
  id: string;
  year: string;
  title: string;
  text: string;
  statusTitle?: string;
  statusText?: string;
  photos: StaticPhoto[];
}

export interface StaticTestimonial {
  authorName: string;
  authorRole: string;
  content: string;
}

function buildEditionPhotos(year: number, count: number): StaticPhoto[] {
  return Array.from({ length: count }, (_, index) => {
    const number = index + 1;
    const label = String(number).padStart(2, '0');
    return {
      caption: `Souvenir FISD ${year} - ${label}`,
      imageUrl: `assets/images/edition-${year}/${number}.jpg`,
      description: `Moment capturé lors de l'édition ${year} du FISD à Montréal.`,
    };
  });
}

export const SLIDES: Slide[] = [
  {
    image: 'assets/images/marraine-fisd-2026.jpg',
    focalPoint: 'center 18%',
    kicker: 'Marraine du FISD 2026',
    title: 'Son Excellence Madame Kandia Kamissoko Camara',
    text: 'Présidente du Sénat de la Côte d’Ivoire. Figure majeure du leadership panafricain, elle soutient l’éducation, la coopération internationale et la diaspora. Son soutien accompagne les actions de sensibilisation et de plaidoyer du FISD en faveur des diasporas.',
    place: 'Côte d’Ivoire',
  },
  {
    image: 'assets/images/parrain-fisd-2026.jpeg',
    focalPoint: 'center 22%',
    kicker: 'Parrain du FISD 2026',
    title: 'Son Excellence le Maréchal Mahamat Idriss Deby Itno',
    text: 'Président de la République du Tchad, Président d’honneur du FISD 2025 et Parrain du FISD 2026.',
    place: 'Tchad',
  },
  {
    image: 'assets/images/sl1.jpeg',
    kicker: 'Prochaine édition du FISD',
    title: 'Édition 2026 : trois jours d’envergure internationale',
    text: "Les 26, 27 et 28 novembre 2026, le Forum International Solidarité et Développement revient pour 3 jours d'immersion, de rencontres et de retombées stratégiques.",
  },
  {
    image: 'assets/images/sl2.jpeg',
    kicker: '26, 27 et 28 novembre 2026',
    title: "Connecter les acteurs d'ici et d'ailleurs",
    text: 'Institutions, diasporas, entrepreneurs, partenaires et acteurs du développement international se retrouveront autour d’une vision commune.',
  },
  {
    image: 'assets/images/sl3.jpeg',
    kicker: 'FISD 2026',
    title: 'Trois jours de rencontres et d’échanges',
    text: 'Panels, rencontres ciblées, réseautage et activités post-forum : retrouvez les acteurs de la solidarité et du développement. Le lieu sera annoncé prochainement.',
  },
];

export const PILLARS: Pillar[] = [
  {
    title: 'Un espace de concertations',
    text: 'Un lieu de dialogue stratégique entre acteurs du développement international, diasporas, ONG et représentants publics.',
    image: 'assets/images/concertations.jpeg',
    icon: '01',
  },
  {
    title: "Un cadre de plaidoyer et d'actions citoyennes",
    text: 'Une plateforme qui met en avant les enjeux sociaux, économiques et humains afin de porter une voix collective.',
    image: 'assets/images/plaidoyer-actions-citoyennes.jpeg',
    icon: '02',
  },
  {
    title: 'Un espace de visibilité et de réseautage',
    text: 'Un environnement propice aux rencontres, aux opportunités et à la mise en valeur des initiatives à fort impact.',
    image: 'assets/images/visibilite-reseautage.jpeg',
    icon: '03',
  },
];

export const PARTICIPANTS: Participant[] = [
  { name: 'Associations', description: 'Diasporas africaines, haïtiennes et communautaires.' },
  { name: 'ONG', description: 'Organismes du secteur du développement international.' },
  { name: 'Universitaires', description: 'Québécois, Canadiens et autres acteurs dans le monde.' },
  {
    name: 'Entrepreneurs',
    description: 'Québécois, Canadiens et membres des diasporas africaines et haïtiennes.',
  },
  { name: 'Diplomates', description: "Du Canada et des pays de l'Afrique de l'Ouest francophone." },
  {
    name: 'Pouvoirs publics',
    description: 'Ministres, députés, élus locaux et leurs représentants.',
  },
];

export const SPEAKERS: Speaker[] = [
  {
    name: 'Mahamat Tochi Chidi',
    role: 'Sénateur du Tchad, rapporteur 2e adjoint de la commission Défense, Sécurité et Souveraineté',
    image: 'assets/images/intervenants/mahamat-tochi-chidi.jpg',
    description: 'Panéliste du FISD 2025, sénateur du Tchad.',
  },
  {
    name: 'Bénie Kouyaté',
    role: 'Présidente de Bénie Foundation Inc.',
    image: 'assets/images/intervenants/benie-kouyate.jpg',
    description: 'Panéliste du FISD 2025, présentée comme présidente de Bénie Foundation Inc.',
  },
  {
    name: 'Hawa Barry Diallo',
    role: 'Présidente de Guinean Women Development',
    image: 'assets/images/intervenants/hawa-barry-diallo.jpg',
    description: 'Panéliste du FISD 2025, présentée comme présidente de Guinean Women Development.',
  },
  {
    name: 'Pr. François Audet',
    role: "Directeur de l'Observatoire canadien sur les crises et l'action humanitaires (OCCAH)",
    image: 'assets/images/intervenants/francois-audet.jpg',
    description: "Panéliste du FISD 2025 et directeur de l'OCCAH.",
  },
  {
    name: 'Catherine Cauchon',
    role: 'Directrice des programmes / Terre Sans Frontières',
    image: 'assets/images/intervenants/catherine-cauchon.jpg',
    description: 'Panéliste du FISD 2025, directrice des programmes chez Terre Sans Frontières.',
  },
  {
    name: 'Alpha Touré',
    role: 'Président du comité administratif de Jeunes Solidaires',
    image: 'assets/images/intervenants/alpha-toure.jpg',
    description:
      'Président du FISD 2025 et président du comité administratif de Jeunes Solidaires.',
  },
  {
    name: 'M. Seydou Togola',
    role: 'Directeur pays / Terre Sans Frontières - Mali',
    image: 'assets/images/intervenants/seydou-togola.jpg',
    description: 'Panéliste du FISD 2025, directeur pays pour Terre Sans Frontières au Mali.',
  },
  {
    name: 'M. Sansy Kaba Diakité',
    role: "Directeur de L'Harmattan Guinée et président-fondateur du Lions Club Conakry Bate",
    image: 'assets/images/intervenants/sansy-kaba-diakite.jpg',
    description:
      "Panéliste du FISD 2025, directeur de L'Harmattan Guinée et président-fondateur du Lions Club Conakry Bate.",
  },
  {
    name: 'Mme Pauline Effa',
    role: "Vice-présidente du Forum International de l'Économie Sociale et Solidaire",
    image: 'assets/images/intervenants/pauline-effa.jpg',
    description:
      "Panéliste du FISD 2025, vice-présidente du Forum International de l'Économie Sociale et Solidaire et cofondatrice du FORAESS.",
  },
  {
    name: 'Dr. Paubert T. Mahatante',
    role: 'Ministre de la Pêche et de l’Économie bleue de Madagascar',
    image: 'assets/images/intervenants/paubert-mahatante.jpg',
    description:
      "Panéliste du FISD 2025, ministre de la Pêche et de l'Économie bleue de Madagascar.",
  },
  {
    name: 'Mme Michèle Asselin',
    role: 'Directrice générale - AQOCI',
    image: 'assets/images/intervenants/michele-asselin.jpg',
    description: "Panéliste du FISD 2025, directrice générale de l'AQOCI.",
  },
  {
    name: 'M. Michel Filion, Ph.D.',
    role: 'Spécialiste des politiques et des finances publiques',
    image: 'assets/images/intervenants/michel-filion.jpg',
    description: 'Panéliste du FISD 2025, spécialiste des politiques et des finances publiques.',
  },
  {
    name: 'Bigsoul 224',
    role: 'Créateur de contenu',
    image: 'assets/images/intervenants/bigsoul-224.jpg',
    description: 'Panéliste du FISD 2025 et créateur de contenu.',
  },
  {
    name: 'Sénateur Abdallah Darkallah Sidi',
    role: "Rapporteur 1er adjoint à la commission des Affaires étrangères et des Tchadiens de l'étranger / Sénat du Tchad",
    image: 'assets/images/intervenants/abdallah-darkallah-sidi.jpg',
    description: 'Panéliste du FISD 2025, sénateur et rapporteur 1er adjoint au Sénat du Tchad.',
  },
];

export const PARTNERS: Partner[] = [
  {
    name: 'UNCCIAS Union Nationale des Chambres de Commerce, d’Industrie et d’Agriculture du Sénégal',
    label: 'UNCCIAS',
    logo: 'assets/logos/partenaires/unccias-horizontal.jpeg',
  },
  {
    name: 'UNCCIAS Union Nationale des Chambres de Commerce, d’Industrie et d’Agriculture du Sénégal',
    label: 'UNCCIAS',
    logo: 'assets/logos/partenaires/unccias-vertical.jpeg',
  },
  { name: 'Ville de Montréal', logo: 'assets/logos/partenaires/montreal.jpg' },
  { name: 'Gouvernement du Québec', logo: 'assets/logos/partenaires/quebec.png' },
  { name: 'FISIQ', logo: 'assets/logos/partenaires/fisiq.png' },
  { name: 'CECI', logo: 'assets/logos/partenaires/ceci.png' },
  { name: 'Educonnexion', logo: 'assets/logos/partenaires/educonnexion.png' },
  { name: 'Village Monde', logo: 'assets/logos/partenaires/village-monde.png' },
  { name: 'SUCO', logo: 'assets/logos/partenaires/suco.png' },
  { name: 'Terre Sans Frontières', logo: 'assets/logos/partenaires/terre-sans-frontieres.png' },
  { name: 'Coopération Canada', logo: 'assets/logos/partenaires/cooperation-canada.png' },
  { name: 'AQOCI', logo: 'assets/logos/partenaires/aqoci.png' },
  { name: 'AEOC', logo: 'assets/logos/partenaires/aeoc.png' },
  { name: 'UQAM IEIM', logo: 'assets/logos/partenaires/uqam-ieim.png' },
  { name: 'Ad Alefa Diaspora', logo: 'assets/logos/partenaires/ad-alefa-diaspora.png' },
  { name: 'Espace Afrique', logo: 'assets/logos/partenaires/espace-afrique.png' },
  { name: 'ComDev Africa', logo: 'assets/logos/partenaires/comdev-africa.png' },
  {
    name: 'Guinean Women Development Foundation',
    logo: 'assets/logos/partenaires/guinean-women.png',
  },
];

export const ENGAGEMENT_ACTIONS: EngagementAction[] = [
  {
    anchor: 'partenaire',
    title: 'Devenir partenaire',
    text: 'Associer votre organisation au FISD et construire une collaboration visible, durable et utile.',
    icon: 'PT',
    image: 'assets/images/agir-avec-nous.jpg',
    link: 'https://docs.google.com/forms/d/1wj2F1RXTcSjzTfX-KGPXceh_wCg-hfQ3LZ5prZvRlxw/viewform?edit_requested=true',
    cta: 'Remplir le formulaire partenaire',
    detail:
      'Pour les institutions, organisations, entreprises ou structures qui souhaitent soutenir le forum.',
  },
  {
    anchor: 'stand',
    title: 'Prendre un stand',
    text: 'Présenter vos initiatives, projets, produits ou services dans un espace dédié pendant le forum.',
    icon: 'ST',
    image: 'assets/images/fisd-est-visibilite.jpg',
    link: 'https://docs.google.com/forms/d/e/1FAIpQLSep06rVkgrzSKk0sCVjhix5B0cRHw_wkrbdQvxIbagHcf4zxA/viewform',
    cta: 'Demander un stand',
    detail:
      'Pour les exposants qui veulent rencontrer le public, les partenaires et les participants.',
  },
  {
    anchor: 'benevole',
    title: 'Devenir bénévole',
    text: "Contribuer à l'accueil, à l'organisation et au bon déroulement des activités du FISD.",
    icon: 'BV',
    image: 'assets/images/agir.jpeg',
    link: 'https://docs.google.com/forms/d/e/1FAIpQLSdOfyKT8_KsmNQYFPWtLFssvd-Yb9d6iKAu7hOPC6kC07qcJQ/viewform',
    cta: "Rejoindre l'équipe bénévole",
    detail:
      'Pour les personnes qui veulent donner du temps et participer concrètement à l’événement.',
  },
];

export const PROGRAMS: Program[] = [
  {
    id: '2025',
    badge: 'Édition 2025',
    title: "Programme officiel de l'édition 2025",
    text: "Préparez-vous à vivre 2 jours d'immersion transformative, suivis d'activités post-forum exclusives, conçues pour maximiser les retombées stratégiques de votre participation au FISD. La prochaine édition se tiendra les 5 et 6 décembre 2025 à Montréal, au Canada.",
    days: [
      {
        id: 'j1',
        label: 'Jour 1',
        date: 'Vendredi 5 décembre 2025',
        schedule: [
          {
            time: '08h00 - 09h00',
            title: 'Accueil et enregistrement des invités',
            tag: 'Accueil',
            detail:
              'Accueil des participants, enregistrement des invités et orientation sur le site du forum.',
            location: 'Hall principal',
          },
          {
            time: '09h00',
            title: "Cérémonie d'ouverture",
            tag: 'Ouverture',
            detail: "Lancement officiel de l'édition 2025 du FISD.",
            location: 'Salle principale',
          },
          {
            time: 'Matin',
            title: 'Panel 1',
            tag: 'Panel',
            detail: "Premier panel de réflexion et d'échanges autour des grands enjeux du forum.",
            location: 'Salle principale',
          },
          {
            time: 'Midi',
            title: 'Pause déjeuner',
            tag: 'Pause',
            detail: "Temps de repas, d'échange libre et de réseautage entre participants.",
            location: 'Espace restauration',
          },
          {
            time: 'Après-midi',
            title: 'Panel 2',
            tag: 'Panel',
            detail:
              'Deuxième panel de la journée avec les intervenants et parties prenantes du forum.',
            location: 'Salle principale',
          },
          {
            time: 'Après-midi',
            title: 'Ateliers thématiques',
            tag: 'Atelier',
            detail: 'Sessions de travail en groupes autour des thèmes stratégiques du FISD.',
            location: 'Salles ateliers',
          },
          {
            time: 'Fin de journée',
            title: 'FISD Connect',
            tag: 'Réseautage',
            detail:
              'Moment de mise en relation entre participants, partenaires, organisations et exposants.',
            location: 'Espace réseautage',
          },
          {
            time: 'En continu',
            title: 'Animation des stands',
            tag: 'Exposition',
            detail:
              'Animation des stands des entreprises, organismes du secteur du développement international, municipalités et autres exposants.',
            location: 'Espace exposants',
          },
        ],
      },
      {
        id: 'j2',
        label: 'Jour 2',
        date: 'Samedi 6 décembre 2025',
        schedule: [
          {
            time: '08h00 - 09h00',
            title: 'Accueil et enregistrement des invités',
            tag: 'Accueil',
            detail:
              'Accueil des participants et enregistrement des invités pour la deuxième journée du forum.',
            location: 'Hall principal',
          },
          {
            time: 'Matin',
            title: 'Connexion solidaire : ONG internationales et diasporas',
            tag: 'Rencontre',
            detail:
              "Moment d'échange entre ONG internationales, diasporas et acteurs du développement.",
            location: 'Salle principale',
          },
          {
            time: 'Matin',
            title: 'Panel 3',
            tag: 'Panel',
            detail: "Troisième panel de l'édition 2025 du FISD.",
            location: 'Salle principale',
          },
          {
            time: 'Midi',
            title: 'Pause déjeuner',
            tag: 'Pause',
            detail: "Temps de repas, d'échange libre et de réseautage.",
            location: 'Espace restauration',
          },
          {
            time: 'Après-midi',
            title: 'Keynote',
            tag: 'Conférence',
            detail:
              'Intervention principale autour des enjeux de solidarité, de coopération et de développement.',
            location: 'Salle principale',
          },
          {
            time: 'Après-midi',
            title: 'Panel 4',
            tag: 'Panel',
            detail: 'Quatrième panel de discussion avec les intervenants invités.',
            location: 'Salle principale',
          },
          {
            time: 'Après-midi',
            title: 'Ateliers thématiques',
            tag: 'Atelier',
            detail: 'Sessions participatives autour des thèmes majeurs du forum.',
            location: 'Salles ateliers',
          },
          {
            time: 'Fin de journée',
            title: 'Présentation des exposants et remerciements des partenaires',
            tag: 'Présentation',
            detail: 'Mise en valeur des exposants et reconnaissance des partenaires du FISD.',
            location: 'Salle principale',
          },
          {
            time: 'Fin de journée',
            title: 'Cérémonie de clôture',
            tag: 'Clôture',
            detail: "Clôture officielle de l'édition 2025 du FISD.",
            location: 'Salle principale',
          },
        ],
      },
      {
        id: 'marge',
        label: 'Activités en marge du FISD',
        date: 'Du 13 au 15 décembre 2025',
        schedule: [
          {
            time: 'Sur rendez-vous',
            title: 'Visites institutionnelles',
            tag: 'Visite',
            detail:
              'Visites prévues pour les dignitaires internationaux qui prendront part au FISD et certains participants, notamment maires, députés et ministres.',
            location: 'Ottawa, Montréal et Québec',
          },
          {
            time: 'Visite 1',
            title: 'Parlement du Canada',
            tag: 'Institutionnel',
            detail: 'Rencontre et visite institutionnelle.',
            location: 'Ottawa',
          },
          {
            time: 'Visite 2',
            title: 'Ministère canadien',
            tag: 'Institutionnel',
            detail: 'Visite à confirmer.',
            location: 'Canada',
          },
          {
            time: 'Visite 3',
            title: 'Ambassade africaine à Ottawa',
            tag: 'Institutionnel',
            detail: 'Visite à confirmer.',
            location: 'Ottawa',
          },
          {
            time: 'Visite 4',
            title: 'Ville de Montréal',
            tag: 'Institutionnel',
            detail: 'Rencontre avec les acteurs municipaux.',
            location: 'Montréal',
          },
          {
            time: 'Visite 5',
            title: 'Municipalités au Québec',
            tag: 'Institutionnel',
            detail: 'Visites à confirmer.',
            location: 'Québec',
          },
          {
            time: 'Sur mesure',
            title: 'Réseautage ciblé',
            tag: 'Réseautage',
            detail: 'Mise en relation ciblée selon les profils et objectifs des participants.',
            location: 'À confirmer',
          },
          {
            time: 'Sur mesure',
            title: 'Agendas sur mesure',
            tag: 'Accompagnement',
            detail:
              'Organisation d’agendas personnalisés pour maximiser les retombées de la participation.',
            location: 'À confirmer',
          },
          {
            time: 'Optionnel',
            title: 'Visites touristiques',
            tag: 'Découverte',
            detail: 'Activités de découverte et visites touristiques.',
            location: 'Montréal et environs',
          },
        ],
      },
    ],
  },
  {
    id: '2026',
    badge: 'Édition 2026',
    title: 'Rendez-vous à Montréal',
    text: "L'édition 2026 du FISD se déroulera les 26, 27 et 28 novembre 2026 à Montréal, au Canada. Préparez-vous à vivre 3 jours d'immersion transformative, suivis d'activités post-forum exclusives, conçues pour maximiser les retombées stratégiques de votre participation au FISD. Le lieu sera confirmé prochainement.",
    days: [
      {
        id: 'avenir',
        label: 'À venir',
        date: '26, 27 et 28 novembre 2026',
        schedule: [
          {
            time: 'À venir',
            title: 'Programme en préparation',
            tag: 'Information',
            detail:
              'Visibilité, partage d’expériences et rencontres : trois journées pour agir ensemble.',
            location: 'À confirmer',
          },
        ],
      },
    ],
  },
];

export const STATIC_EVENTS: StaticEvent[] = [
  {
    id: '2024',
    year: 'Édition 2024',
    title: 'Rencontres et collaborations',
    text: 'Une édition orientée vers les initiatives citoyennes.',
    photos: buildEditionPhotos(2024, 62),
  },
  {
    id: '2025',
    year: 'Édition 2025',
    title: 'Moments forts du forum',
    text: 'Panels, conférences, rencontres institutionnelles et échanges.',
    photos: buildEditionPhotos(2025, 82),
  },
  {
    id: '2026',
    year: 'Édition 2026',
    title: 'À venir',
    text: "Les souvenirs de l'édition 2026 seront ajoutés ici après l'événement.",
    statusTitle: 'À venir',
    statusText:
      "Les photos et moments forts de l'édition 2026 seront publiés ici dès qu'ils seront disponibles.",
    photos: [],
  },
];

export const STATIC_TESTIMONIALS: StaticTestimonial[] = [
  {
    authorName: 'Marie Veillette',
    authorRole: 'Caravane Philanthrope',
    content:
      'Merci infiniment aux organisatrices et organisateurs de ce forum ! Quel bel événement rempli d’apprentissage et d’échanges enrichissants. Pour un organisme comme le nôtre Caravane Philanthrope qui vise des pratiques éthiques en coopération internationale, ce genre de moment est vraiment précieux. J’espère que ce ne sera pas le dernier et que d’autres occasions comme celle-ci continueront d’inspirer et de rassembler. Chapeau pour votre merveilleuse organisation.',
  },
  {
    authorName: 'Benie Kouyate',
    authorRole: 'Participante',
    content:
      "Je suis particulièrement reconnaissante d'avoir eu l'opportunité de partager mes idées et d'échanger avec des participants aussi passionnés et engagés. Je vous remercie également pour l'accueil chaleureux et l'organisation remarquable de cet événement. Je suis convaincue que le FISD continuera à jouer un rôle crucial dans la promotion de la solidarité et du développement à l'échelle internationale.",
  },
  {
    authorName: 'Valérie Phaneuf',
    authorRole: 'Participante',
    content:
      "Merci pour cette belle opportunité de mettre de l'avant des entrepreneurs sociaux et de discuter de grands enjeux de coopération internationale ! Quel bel événement ! Merci à tous les organisateurs et participants ! Merci Alpha Touré pour l'invitation !",
  },
];

export const RESERVATION_TYPES: string[] = [
  'Participant',
  'Partenaire',
  'Invité',
  'Exposant',
  'Bénévole',
];
