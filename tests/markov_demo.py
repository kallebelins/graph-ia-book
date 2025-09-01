import numpy as np

def fundamental_matrix(Q: np.ndarray) -> np.ndarray:
    I=np.eye(Q.shape[0])
    return np.linalg.inv(I-Q)

def main():
    # Exemplo simples com 2 estados transientes e 1 absorvente
    Q=np.array([[0.5,0.3],[0.2,0.6]], dtype=float)
    R=np.array([[0.2],[0.2]], dtype=float)
    N=fundamental_matrix(Q)
    ones=np.ones((Q.shape[0],1))
    t=N@ones
    B=N@R
    np.set_printoptions(precision=4, suppress=True)
    print('N=\n',N)
    print('t=\n',t.ravel())
    print('B=\n',B.ravel())

if __name__=='__main__':
    main()

