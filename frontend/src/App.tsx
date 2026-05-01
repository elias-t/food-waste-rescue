import { Routes, Route, Navigate, Outlet } from 'react-router-dom';
import { useAuth } from './store/useAuth';
import { type UserRole } from './types/api';

// Layout component — shared header/footer (we'll flesh out later)
function Layout() {
  return (
    <div className="min-h-screen">
      {/* Header will go here */}
      <main>
        <Outlet />   {/* child routes render here */}
      </main>
    </div>
  );
}

// ProtectedRoute — guards routes by auth + optional role
function ProtectedRoute({ requiredRole }: { requiredRole?: UserRole }) {
  const { isAuthenticated, user } = useAuth();
  
  if (!isAuthenticated) return <Navigate to="/login" replace />;
  if (requiredRole && user?.role !== requiredRole) return <Navigate to="/" replace />;
  
  return <Outlet />;
}

// Placeholder pages — we'll build them properly next chunk
function HomePage() {
  return <div className="p-8 text-center">
    <h1 className="text-3xl font-bold text-primary">Food Waste Rescue</h1>
  </div>;
}
function LoginPage() { return <div className="p-8">Login (placeholder)</div>; }
function RegisterPage() { return <div className="p-8">Register (placeholder)</div>; }
function NearbyListingsPage() { return <div className="p-8">Nearby listings (placeholder)</div>; }
function ListingDetailPage() { return <div className="p-8">Listing detail (placeholder)</div>; }
function MyListingsPage() { return <div className="p-8">My listings (placeholder)</div>; }
function CreateListingPage() { return <div className="p-8">Create listing (placeholder)</div>; }

function App() {
  return (
    <Routes>
      <Route path="/" element={<Layout />}>
        {/* Public routes */}
        <Route index element={<HomePage />} />
        <Route path="login" element={<LoginPage />} />
        <Route path="register" element={<RegisterPage />} />
        <Route path="listings" element={<NearbyListingsPage />} />
        <Route path="listings/:id" element={<ListingDetailPage />} />
        
        {/* Donor-only protected routes */}
        <Route element={<ProtectedRoute requiredRole="Donor" />}>
          <Route path="my-listings" element={<MyListingsPage />} />
          <Route path="create-listing" element={<CreateListingPage />} />
        </Route>
        
        {/* Catch-all */}
        <Route path="*" element={<Navigate to="/" replace />} />
      </Route>
    </Routes>
  );
}

export default App;