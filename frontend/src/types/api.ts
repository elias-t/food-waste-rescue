// ============================================
// Enums (string unions — preferred in modern TS)
// ============================================

export type FoodCategory =
  | 'Bakery'
  | 'Dairy'
  | 'FruitsAndVegetables'
  | 'MeatAndFish'
  | 'PreparedMeals'
  | 'Pantry'
  | 'Beverages'
  | 'Other';

export type ListingStatus =
  | 'Active'
  | 'Claimed'
  | 'Expired'
  | 'Cancelled';

export type ClaimStatus =
  | 'Pending'
  | 'Confirmed'
  | 'Collected'
  | 'Cancelled';

export type UserRole =
  | 'Donor'
  | 'Claimer'
  | 'Admin';

// ============================================
// Auth
// ============================================

export interface AuthResponse {
  token: string;
}

export interface LoginRequest {
  email: string;
  password: string;
}

export interface RegisterRequest {
  email: string;
  password: string;
  displayName: string;
  organisationName?: string;
  role: UserRole;
}

// ============================================
// Listings
// ============================================

export interface NearbyListing {
  id: string;
  title: string;
  category: FoodCategory;
  quantityDescription: string;
  expiresAt: string;        // ISO 8601 — convert to Date when displaying
  address: string | null;
  latitude: number | null;
  longitude: number | null;
  donorName: string;
  distanceKm: number;
}

export interface CreateListingRequest {
  title: string;
  description: string;
  category: FoodCategory;
  quantityDescription: string;
  expiresAt: string;        // ISO 8601
  latitude?: number;
  longitude?: number;
  address?: string;
}

export interface ListingDetail {
  id: string;
  title: string;
  description: string;
  category: FoodCategory;
  quantityDescription: string;
  expiresAt: string;
  status: ListingStatus;
  address: string | null;
  latitude: number | null;
  longitude: number | null;
  donorName: string;
  organisationName: string | null;
  claimCount: number;
  createdAt: string;
}

export interface MyListing {
  id: string;
  title: string;
  category: FoodCategory;
  status: ListingStatus;
  expiresAt: string;
  claimCount: number;
  createdAt: string;
}