import csv
import os

def main():
    base=os.path.dirname(__file__)
    csv_path=os.path.join(os.path.dirname(base), 'data', 'makespan_cases.csv')
    with open(csv_path, newline='' , encoding='utf-8') as f:
        reader=csv.DictReader(f)
        print('case_id,T_chain,T_grafo')
        for row in reader:
            times=[float(v) for k,v in row.items() if k.startswith('t') and k!='t_agg']
            t_agg=float(row.get('t_agg',0.0))
            t_chain=sum(times)
            t_grafo=max(times)+t_agg
            print(f'{row['case_id']},{t_chain:.2f},{t_grafo:.2f}')

if __name__=='__main__':
    main()

