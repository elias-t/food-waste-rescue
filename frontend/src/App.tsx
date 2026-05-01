import { Routes, Route, Navigate, Outlet, Link, useNavigate } from 'react-router-dom';
import { LogOut, User, Leaf } from 'lucide-react';
import { useAuth } from './store/useAuth';
import { type UserRole } from './types/api';
import LoginPage from './pages/LoginPage';
import RegisterPage from './pages/RegisterPage';

function Header() {
  const { isAuthenticated, user, logout } = useAuth();
  const navigate = useNavigate();

  const handleLogout = () => {
    logout();
    navigate('/');
  };

  return (
    <header className="sticky top-0 z-50 bg-white border-b border-gray-200">
      <div className="mx-auto max-w-6xl px-4 h-16 flex items-center justify-between">
        <Link to="/" className="flex items-center gap-2 font-bold text-primary text-lg">
          <Leaf className="h-5 w-5" />
          Food Waste Rescue
        </Link>

        <nav className="flex items-center gap-4">
          {isAuthenticated && user ? (
            <>
              <span className="flex items-center gap-1.5 text-sm text-gray-600">
                <User className="h-4 w-4" />
                {user.displayName}
              </span>
              <button
                onClick={handleLogout}
                className="flex items-center gap-1.5 text-sm text-gray-500 hover:text-gray-900 transition-colors"
              >
                <LogOut className="h-4 w-4" />
                Logout
              </button>
            </>
          ) : (
            <>
              <Link
                to="/login"
                className="text-sm text-gray-600 hover:text-gray-900 transition-colors"
              >
                Sign in
              </Link>
              <Link
                to="/register"
                className="text-sm bg-primary text-white px-4 py-2 rounded-lg hover:bg-primary-dark transition-colors"
              >
                Register
              </Link>
            </>
          )}
        </nav>
      </div>
    </header>
  );
}

function Layout() {
  return (
    <div className="min-h-screen bg-gray-50">
      <Header />
      <main className="mx-auto max-w-6xl px-4 py-6">
        <Outlet />
      </main>
    </div>
  );
}

function ProtectedRoute({ requiredRole }: { requiredRole?: UserRole }) {
  const { isAuthenticated, user } = useAuth();

  if (!isAuthenticated) return <Navigate to="/login" replace />;
  if (requiredRole && user?.role !== requiredRole) return <Navigate to="/" replace />;

  return <Outlet />;
}

function HomePage() {
  return (
    <div className="p-8 text-center">
      <h1 className="text-3xl font-bold text-primary">Food Waste Rescue</h1>
    </div>
  );
}

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
