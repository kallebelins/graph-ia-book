import random
from typing import List, Tuple


def generate_graph(num_nodes: int = 12, seed: int = 42) -> Tuple[List[Tuple[int, int]], List[float]]:
    random.seed(seed)
    edges = []
    # Simple layered DAG generator
    layers = [list(range(0, 4)), list(range(4, 8)), list(range(8, 12))]
    for u in layers[0]:
        v = random.choice(layers[1])
        edges.append((u, v))
    for u in layers[1]:
        v = random.choice(layers[2])
        edges.append((u, v))
    # synthetic node latencies (label) and degree features
    latencies = [0.2 + random.random() * 0.6 for _ in range(num_nodes)]
    return edges, latencies


def main():
    edges, latencies = generate_graph()
    print("edges:", edges)
    print("latencies:", [round(x, 3) for x in latencies])
    # Placeholder: in a real setup, feed edges/features to a GNN lib (e.g., PyG/DGL)
    print("note: this is a synthetic stub to illustrate data preparation for the pilot.")


if __name__ == "__main__":
    main()


