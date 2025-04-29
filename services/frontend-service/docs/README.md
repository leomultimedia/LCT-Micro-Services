# Frontend Service Documentation

## Overview
The Frontend Service provides the user interface for the E-Commerce platform, built with React and integrated with the backend microservices.

## Architecture
```mermaid
graph TB
    subgraph Frontend Service
        subgraph Core Features
            UI[User Interface]
            Auth[Authentication]
            Cart[Shopping Cart]
            Checkout[Checkout]
        end

        subgraph Components
            Layout[Layout]
            Pages[Pages]
            Components[Components]
            Hooks[Hooks]
        end

        subgraph Integration
            AG[API Gateway]
            Auth[Auth Service]
            PS[Product Service]
            OS[Order Service]
        end
    end

    UI --> Layout
    Auth --> Hooks
    Cart --> Components
    Checkout --> Pages

    UI --> AG
    Auth --> Auth
    Cart --> PS
    Checkout --> OS
```

## Component Structure

### Layout Components
```typescript
interface LayoutProps {
  children: React.ReactNode;
  header?: React.ReactNode;
  footer?: React.ReactNode;
}

const Layout: React.FC<LayoutProps> = ({ children, header, footer }) => (
  <div className="layout">
    {header && <header>{header}</header>}
    <main>{children}</main>
    {footer && <footer>{footer}</footer>}
  </div>
);
```

### Page Components
```typescript
interface ProductPageProps {
  productId: string;
}

const ProductPage: React.FC<ProductPageProps> = ({ productId }) => {
  const { data: product, loading } = useProduct(productId);
  
  if (loading) return <LoadingSpinner />;
  
  return (
    <Layout>
      <ProductDetails product={product} />
      <AddToCart productId={productId} />
    </Layout>
  );
};
```

## State Management

### Redux Store
```typescript
interface AppState {
  auth: AuthState;
  cart: CartState;
  products: ProductsState;
  orders: OrdersState;
}

const rootReducer = combineReducers({
  auth: authReducer,
  cart: cartReducer,
  products: productsReducer,
  orders: ordersReducer
});
```

### Custom Hooks
```typescript
const useCart = () => {
  const dispatch = useDispatch();
  const cart = useSelector((state: AppState) => state.cart);

  const addToCart = (productId: string) => {
    dispatch(addToCartAction(productId));
  };

  return { cart, addToCart };
};
```

## API Integration

### API Client
```typescript
const apiClient = axios.create({
  baseURL: process.env.REACT_APP_API_URL,
  headers: {
    'Content-Type': 'application/json'
  }
});

apiClient.interceptors.request.use((config) => {
  const token = localStorage.getItem('token');
  if (token) {
    config.headers.Authorization = `Bearer ${token}`;
  }
  return config;
});
```

### Service Integration
```typescript
const productService = {
  getProducts: async () => {
    const response = await apiClient.get('/products');
    return response.data;
  },
  
  getProduct: async (id: string) => {
    const response = await apiClient.get(`/products/${id}`);
    return response.data;
  }
};
```

## Configuration

### Environment Variables
```env
REACT_APP_API_URL=http://localhost:5007
REACT_APP_AUTH_URL=https://your-tenant.b2clogin.com
REACT_APP_CLIENT_ID=your-client-id
```

### Build Configuration
```json
{
  "scripts": {
    "start": "react-scripts start",
    "build": "react-scripts build",
    "test": "react-scripts test",
    "eject": "react-scripts eject"
  },
  "dependencies": {
    "react": "^18.2.0",
    "react-dom": "^18.2.0",
    "react-router-dom": "^6.0.0",
    "redux": "^4.0.0",
    "axios": "^1.0.0"
  }
}
```

## Routing

### Route Configuration
```typescript
const routes = [
  {
    path: '/',
    element: <Layout />,
    children: [
      { path: '', element: <HomePage /> },
      { path: 'products', element: <ProductsPage /> },
      { path: 'products/:id', element: <ProductPage /> },
      { path: 'cart', element: <CartPage /> },
      { path: 'checkout', element: <CheckoutPage /> },
      { path: 'orders', element: <OrdersPage /> }
    ]
  }
];
```

### Protected Routes
```typescript
const ProtectedRoute: React.FC<{ children: React.ReactNode }> = ({ children }) => {
  const { isAuthenticated } = useAuth();
  
  if (!isAuthenticated) {
    return <Navigate to="/login" />;
  }
  
  return <>{children}</>;
};
```

## Development

### Prerequisites
- Node.js 16+
- npm or yarn
- React 18+
- TypeScript 4+

### Setup
1. Install dependencies:
   ```bash
   npm install
   # or
   yarn install
   ```

2. Configure environment:
   ```bash
   cp .env.example .env
   # Edit .env with your configuration
   ```

3. Start development server:
   ```bash
   npm start
   # or
   yarn start
   ```

## Testing

### Unit Tests
```typescript
describe('ProductPage', () => {
  it('renders product details', () => {
    const { getByText } = render(<ProductPage productId="1" />);
    expect(getByText('Product Details')).toBeInTheDocument();
  });
});
```

### Integration Tests
```typescript
describe('Cart Integration', () => {
  it('adds product to cart', async () => {
    const { getByText } = render(<App />);
    fireEvent.click(getByText('Add to Cart'));
    expect(await screen.findByText('Item added to cart')).toBeInTheDocument();
  });
});
```

## Styling

### CSS Modules
```css
/* ProductCard.module.css */
.card {
  border: 1px solid #ddd;
  border-radius: 4px;
  padding: 1rem;
}

.title {
  font-size: 1.2rem;
  margin-bottom: 0.5rem;
}

.price {
  color: #666;
  font-weight: bold;
}
```

### Theme Configuration
```typescript
const theme = {
  colors: {
    primary: '#007bff',
    secondary: '#6c757d',
    success: '#28a745',
    danger: '#dc3545'
  },
  spacing: {
    xs: '0.25rem',
    sm: '0.5rem',
    md: '1rem',
    lg: '1.5rem'
  }
};
```

## Performance

### Code Splitting
```typescript
const ProductPage = lazy(() => import('./pages/ProductPage'));
const CartPage = lazy(() => import('./pages/CartPage'));
```

### Caching
```typescript
const { data: products } = useQuery('products', fetchProducts, {
  staleTime: 5 * 60 * 1000,
  cacheTime: 30 * 60 * 1000
});
```

## Troubleshooting

### Common Issues
1. **Authentication**
   - Check token storage
   - Verify redirect URIs
   - Monitor session state

2. **API Integration**
   - Check network requests
   - Verify CORS configuration
   - Monitor error responses

3. **State Management**
   - Check Redux store
   - Verify action dispatching
   - Monitor state updates

4. **Performance**
   - Monitor bundle size
   - Check render performance
   - Optimize images

## Support
- [React Documentation](https://reactjs.org/docs)
- [Redux Documentation](https://redux.js.org/)
- [React Router Documentation](https://reactrouter.com/docs)
- [Issue Tracking](.github/ISSUE_TEMPLATE.md)

<div align="center">
  <p>
    <em>© 2024 Lear Cyber Tech. All rights reserved.</em>
  </p>
</div> 